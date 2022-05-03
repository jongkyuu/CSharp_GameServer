﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Session
    {
        Socket _socket;
        int _disconnected = 0;

        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();

        public void Start(Socket socket)
        {
            _socket = socket;
            // recive를 비동기 방식으로 변경
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);  // _socket.ReceiveAsync가 나중에 성공하면 이벤트를 통해 콜백으로 실행됨
            recvArgs.SetBuffer(new byte[1024], 0, 1024);

            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);  // _socket.ReceiveAsync가 나중에 성공하면 이벤트를 통해 콜백으로 실행됨

            RegisterRecv(recvArgs);
        }

        public void Send(byte[] sendBuff)
        {
            // 아직 Blocking 함수 사용
            //_socket.Send(sendBuff);

            _sendArgs.SetBuffer(sendBuff, 0, sendBuff.Length);
            RegisterSend();
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1) // 이전 값이 1이면 return
                return;
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        void RegisterSend()
        {
            bool pending = _socket.SendAsync(_sendArgs);  // 운영체제가 커널단에서 처리하기 때문에 아무렇게나 호출하면 안됨
            if (pending == false)
                OnSendCompleted(null, _sendArgs);
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {

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

        #region 네트워크 통신
        void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);
            if (pending == false)  // 운 좋게 바로 데이터를 리턴받는 경우
                OnRecvCompleted(null, args);
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            // 경우에 따라 0 byte가 올수도 있음(상대가 연결을 끊는다거나 할때) 
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    // TODO, 나중에는 간단하게 string만 받아오지 않고 복잡하게 바꿀 예정
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);  // args.BytesTransferred : 몇 byte를 받았는지
                    Console.WriteLine($"[From Client] : {recvData}");

                    RegisterRecv(args);
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
