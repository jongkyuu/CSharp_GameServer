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
    class Knight
    {
        public int hp;
        public int attack;
        public string name;
        public List<int> skills = new List<int>();
    }

    // ServerEngine과 Server Contents의 분리
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            // 보내는 순간에 외부에서 bytes를 만들어 줬다
            // 지금은 간단하게 문자열을 byte 배열로 만들었는데
            // 실제 게임에서는 패킷이라는 정보를 만들어서 보냄

            Knight knight = new Knight() { hp = 100, attack = 10 };
            byte[] sendBuffer = new byte[1024];
            byte[] buffer = BitConverter.GetBytes(knight.hp);
            byte[] buffer2 = BitConverter.GetBytes(knight.attack);
            Array.Copy(buffer, 0, sendBuffer, 0, buffer.Length);
            Array.Copy(buffer2, 0, sendBuffer, buffer.Length, buffer2.Length);

            byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server");
            Send(sendBuff);

            Thread.Sleep(1000);

            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);  // args.BytesTransferred : 몇 byte를 받았는지
            Console.WriteLine($"[From Client] : {recvData}");
            return buffer.Count;

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
