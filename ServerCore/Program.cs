using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        // Thread의 메인 함수
        static void MainThread(object state)
        {
            //while(true)
            //    Console.WriteLine("Hello Thread!");

            for (int i=0; i<5; i++)
            {
                Console.WriteLine("Hello Thread!");
            }
        }

        static void Main(string[] args)
        {
            //Thread t = new Thread(MainThread);
            //t.Name = "Test Thread";
            //t.IsBackground = true;
            //t.Start();
            //Console.WriteLine("Waiting For Thread");
            //t.Join();


            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            // 시간이 오래 걸리는 작업이 Thread를 잡고 있으면 전체 시스템이 먹통이 될 수 있음
            //for (int i = 0; i < 4; i++)
            //    ThreadPool.QueueUserWorkItem((obj) => { while (true) { } });  


            for (int i = 0; i < 5; i++)
            {
                Task t = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning); // Work ThreadPool에서 Thread를 뽑아서 사용하는게 아니라 별도의 Thread를 사용함
                t.Start();
            }


            Console.WriteLine("Hello World!");

            ThreadPool.QueueUserWorkItem(MainThread);


            while (true)
            {

            }
        }



    }
}
