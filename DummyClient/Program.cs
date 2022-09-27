using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{


    class Program
    {
        static void Main(string[] args)
        {
            
            // IP 주소 생성
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 7777);

            Connector connector = new Connector();

            connector.Connect(endPoint, () => { return SessionManager.Instacne.Generate(); },
                10);

            //Thread.Sleep(250);

            bool _isInit = true;

            while (true)
            {

                try
                {
                    // 모든 세션들이 서버쪽으로 채팅 메세지를 날려줌
                    SessionManager.Instacne.SendForEach();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Thread.Sleep(250); // 일반적으로 MMO에서 이동 패킷을 1초에 4번정도 보냄.
            }

        }
    }
}
