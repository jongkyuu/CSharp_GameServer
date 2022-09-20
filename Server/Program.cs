using ServerCore;
using System;
using System.Net;

namespace Server
{

    class Program
    {
        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            // www.google.com -> 123.123.123.1
            //PacketManager.Instance.Register(); // single Thread일때 Register 실행

            // IP 주소 생성
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 7777);

            // 문지기(가 들고있는 휴대폰) 생성
            _listener.Init(endPoint, () => { return SessionManager.Instacne.Generate(); });
            Console.WriteLine("Listening...");

            while (true)
            {

            }

        }
    }
}
