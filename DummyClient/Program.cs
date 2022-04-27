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



            while (true)
            {
                // 휴대폰 설정
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // 문지기에게 입장 문의
                    socket.Connect(endPoint);  // Blocking 함수
                    Console.WriteLine($"Connect To {socket.RemoteEndPoint.ToString()}");

                    for(int i=0; i < 5; i++)
                    {
                        // 보낸다
                        byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World {i}");
                        int sendBytes = socket.Send(sendBuff); // Blocking 함수
                    }


                    // 받는다
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = socket.Receive(recvBuff); // Blocking 함수
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[FromServer] : {recvData}");

                    // 나간다
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Thread.Sleep(1000);
            }

        }
    }
}
