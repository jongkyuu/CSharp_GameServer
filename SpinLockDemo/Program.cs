using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpinLockDemo
{
    class SpinLock_1
    {
        volatile bool _locked = false; // 가시성 보장

        public void Acquire()
        {
            while(_locked)
            {
                // 잠금이 풀리기를 기다림
            }

            _locked = true;

        }
        
        public void Release()
        {
            _locked = false;
        }
    }

    class SpinLock_2
    {
        // 문 안으로 들어가서 열쇠로 잠그는 행위가 둘로 나눠지면 안된다 - 원자성
        // 원자성을 보장하여 동시에 들어갈 수 있는 상황을 원천적으로 차단해야 한다

        volatile int _locked = 0; 

        public void Acquire()
        {
            while (true)
            {
                int original = Interlocked.Exchange(ref _locked, 1);
                if (original == 0)
                    break;
            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }


    class SpinLock_3
    {
        volatile int _locked = 0;

        public void Acquire()
        {
            while (true)
            {
                // CAS : Compare-And-Swap
                int expected = 0;
                int desired = 1;
                int original = Interlocked.CompareExchange(ref _locked, desired, expected);
                if (original == 0)
                    break;
            }
        }

        public void Release()
        {
            // 이미 Acquire를 통해 _locked를 유일하게 물고 있는 상황이 되므로 Release에서 문을 열어주는 작업은 별도의 작업 없이 0을 넣어주면 된다.
            _locked = 0;
        }

    }

    class Program
    {
        static int _num = 0;
        //static SpinLock_1 _lock = new SpinLock_1();
        //static SpinLock_2 _lock = new SpinLock_2();
        static SpinLock_3 _lock = new SpinLock_3();

        static void Thread_1()
        {
            for (int i=0; i < 1000000; i++)
            {
                _lock.Acquire();
                _num++;
                _lock.Release();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 1000000; i++)
            {
                _lock.Acquire();
                _num--;
                _lock.Release();
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);
        }
    }
}
