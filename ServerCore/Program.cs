using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        // 하드웨어 최적화

        // 메모리 베리어
        // - 코드 재배치 억제
        // - 가시성

        static int x = 0;
        static int y = 0;
        static int r1 = 0;
        static int r2 = 0;

        static void Thread_1()
        {
            // 실행 순서를 마음대로 뒤바꿔서 문제가 됨
            y = 1;

            // ====================================  MemoryBarrier를 사용해 하드웨어가 실행 순서를 마음대로 조절할 수 없음
            //Thread.MemoryBarrier();
            
            r1 = x;

        }
        static void Thread_2()
        {
            x = 1; 
            // ====================================
            //Thread.MemoryBarrier();
            r2 = y;

        }

        static void Main(string[] args)
        {
            int count = 0;

            while(true)
            {
                count++;
                x = y = r1 = r2 = 0;

                Task t1 = new Task(Thread_1);
                Task t2 = new Task(Thread_2);

                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                if (r1 == 0 && r2 == 0)
                    break;
            }

            Console.WriteLine($"{count}번 만에 빠져나옴");

            Example ex = new Example();
            Task t3 = new Task(ex.A);
            Task t4 = new Task(ex.B);

            t3.Start();
            t4.Start();

            Task.WaitAll(t3, t4);



        }
    }

    class Example
    {
        int _answer;
        bool _complete;

        public void A()
        {
            _answer = 123;
            Thread.MemoryBarrier();
            _complete = true;
            Thread.MemoryBarrier();

        }

        public void B()
        {
            Thread.MemoryBarrier();
            if (_complete)
            {
                Thread.MemoryBarrier();
                Console.WriteLine(_answer);
            }
        }
    }
}
