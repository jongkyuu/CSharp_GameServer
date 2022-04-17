using System;
using System.Threading;
using System.Threading.Tasks;

namespace InterlockedDemo
{
    class Program
    {
        static volatile int number = 0;

        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
            {
                //number++;

                //int temp = number; // 0
                //temp += 1;  // 1
                //number = temp; // number = 1

                // 1 증가시키거나 감소시키는 연산을 한번에 수행해야 하는데 3단계에 걸쳐서 수행하기 때문에 문제가 생긴다
                // atomic : 원자성, 어떤 동작이 한번에 일어나야 한다

                Interlocked.Increment(ref number);
            }


        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                //number--;

                //int temp = number; // 0
                //temp -= 1;  // -1
                //number = temp;  // number = -1

                Interlocked.Decrement(ref number);
            }
        }
        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number) ;


        }
    }
}
