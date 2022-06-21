using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;
using System.Collections.Generic;

namespace Server
{
    class Packet
    {
        public ushort size;  // ushort는 2byte
        public ushort packetId;
        // int로 할때보다 4byte를 아낄 수 있는데 
        // 패킷을 만명의 user에게 보낸다고 하면 4만 byte를 아낄 수 있다 (나비효과)
        // 패킷을 설계할 때는 최대한 압축을 해서 보내는게 중요하다
    }

    // 패킷 사이즈가 유동적으로 변할 수 있음 
    class LoginOkPacket : Packet
    {
        // 캐릭터의 모든 정보를 List로 보낸다고 하면 
        // LoginOkPacket의 사이즈를 쉽게 알 수 없다
        // 그래서 첫 인자로 size를 넘겨줌
    }

    // ServerEngine과 Server Contents의 분리
    class GameSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            // 보내는 순간에 외부에서 bytes를 만들어 줬다
            // 지금은 간단하게 문자열을 byte 배열로 만들었는데
            // 실제 게임에서는 패킷이라는 정보를 만들어서 보냄

            //// TCP는 패킷이 잘려서 올 수 있다
            //Packet packet = new Packet() { size = 100, packetId = 10 };

            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);

            // 100명의 유저가 움직인다면 Send를 하는 횟수가 빈번해짐
            // sendBuff는 세션 내부보다는 외부에서 만들어서 보내주는게 효율적이다
            //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server");
            //Send(sendBuff);

            Thread.Sleep(5000);

            //Disconnect();
        }
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"RecvPacketId : {id}, Size : {size}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }


        // 이후에는 문자열이 아니라 약속된 프로토콜 대로 데이터를 주고 받는다
        // 이동 패킷 (3,2) 좌표로 이동하고 싶다!
        // 15 3 2
        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred Bytes : {numOfBytes}");
        }
    }

    class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            // www.google.com -> 123.123.123.1

            // IP 주소 생성
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 7777);

            // 문지기(가 들고있는 휴대폰) 생성
            _listener.Init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening...");

            while (true)
            {

            }

        }
    }
}
