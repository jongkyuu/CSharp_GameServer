using System;
using System.Threading;
using System.Threading.Tasks;

namespace LockDemo
{
    class Program
    {
        private static int number = 0;
        private static object _obj = new object();

        static void Thread_1(){
            for (int i = 0; i < 1000000; i++)
            {

                // 상호 배제(mutual exclusion, Mutex, 뮤텍스)
                Monitor.Enter(_obj);  // 문을 잠그는 행위
                {
                    try
                    {
                        Monitor.Enter(_obj); // 직접 Monitor를 사용하는 경우는 거의 없다
                        number++;

                    }
                    finally
                    {
                        Monitor.Exit(_obj);
                    }
                    //return;
                    // Exception 발생 등..
                    // 잠금을 열어주지 않고 나갈 경우에 문이 계속 잠겨있기 때문에 Thread_2는 계속 기다리게 된다. (DeadLock)

                }
                //Monitor.Exit(_obj);  // 잠금을 풀어준다
            }
        }

        static void Thread_UseLock_1()
        {
            for (int i = 0; i < 1000000; i++)
            {
                lock (_obj)
                {
                    number++;
                }
         
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 1000000; i++)
            {
                Monitor.Enter(_obj);

                number--;

                Monitor.Exit(_obj);
            }
        }

        static void Thread_UseLock_2()
        {
            for (int i = 0; i < 1000000; i++)
            {
                lock (_obj)
                {
                    number--;
                }

            }
        }

        static void Main(string[] args)
        {
            //Task t1 = new Task(Thread_1);
            //Task t2 = new Task(Thread_2);

            Task t1 = new Task(Thread_UseLock_1);
            Task t2 = new Task(Thread_UseLock_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);

        }
    }
}
