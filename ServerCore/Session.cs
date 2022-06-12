using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    // [size(2)][packetId(2)][...][size(2)][packetId(2)][...] 
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;

            while (true)
            {
                // 최소한 헤더는 파싱할 수 있는지 확인
                if (buffer.Count >= HeaderSize)
                    break;

                // 패킷이 완전체로 도착했는지 확인
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                    break;

                // 여기까지 왔으면 패킷 조립 가능
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                // buffer.Slice(dataSize); 사용 가능

                processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }

            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);


    }
    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(1024);

        object _lock = new object();
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        // send 할때마다 매번 _sendArgs를 생성하는게 아니라 재사용
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        public void Start(Socket socket)
        {
            _socket = socket;
            
            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);  // _socket.ReceiveAsync가 나중에 성공하면 이벤트를 통해 콜백으로 실행됨
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);  // _socket.ReceiveAsync가 나중에 성공하면 이벤트를 통해 콜백으로 실행됨

            RegisterRecv();
        }

        // 멀티스레딩 환경에서 Send를 호출하면 _sendArgs 동일한 이벤트를 사용하는게 문제가 됨. 
        // 기존에 넣어준게 완료되지 않은 상태에서 버퍼가 다른 데이터로 변경되면 에러 발생
        public void Send(ArraySegment<byte> sendBuff)
        {
            lock(_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1) // 이전 값이 1이면 return
                return;

            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        void RegisterSend()
        {
            // 아래 코드는 _sendQueue에 있는 내용을 한꺼번에 보내고 비움 
            // 짧은 시간동안 몇 byte를 보냈는지 추적해서 너무 많이 보낸다면 좀 쉬면서 보내줘야할 필요가 있다
            // 동시에 패킷이 몰릴때 상대가 받을 수 없는데 계속 보내는건 문제가 있음 

            // MultiThread를 고려해서 생각해야한다
            while (_sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                _pendingList.Add(buff);
                //_pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }

            _sendArgs.BufferList = _pendingList;

            bool pending = _socket.SendAsync(_sendArgs);  // 운영체제가 커널단에서 처리하기 때문에 아무렇게나 호출하면 안됨
            if (pending == false)
                OnSendCompleted(null, _sendArgs);
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null; // 굳이 해줄필요는 없음 
                        _pendingList.Clear();

                        OnSend(_sendArgs.BytesTransferred);
                        
                        if (_sendQueue.Count > 0)
                            RegisterSend();
                        //else
                        //    _pending = false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        #region 네트워크 통신
        void RegisterRecv()
        {
            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (pending == false)  // 운 좋게 바로 데이터를 리턴받는 경우
                OnRecvCompleted(null, _recvArgs);
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            // 경우에 따라 0 byte가 올수도 있음(상대가 연결을 끊는다거나 할때) 
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                // TODO, 나중에는 간단하게 string만 받아오지 않고 복잡하게 바꿀 예정
                try
                {
                    // Write커서 이동
                    if(_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    // 컨텐츠쪽으로 데이터를 넘겨주고 얼마나 처리했는지 받는다 
                    //OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));
                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if (processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    // Read 커서 이동
                    if(_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }
                    RegisterRecv();
                }

                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }

            }
            else
            {
                // TODO Disconnect
                Disconnect();
            }
        }

        #endregion
    }
}
