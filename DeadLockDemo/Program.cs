using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeadLockDemo
{
    class SessionManager
    {
        static object _lock = new object();
        public static void TestSession()
        {
            lock (_lock)
            {

            }
        }

        public static void Test()
        {
            lock (_lock)
            {
                UserManager.TestUser();
            }
        }
    }

    class UserManager
    {
        static object _lock = new object();
        public static void TestUser()
        {
            lock (_lock)
            {

            }
        }

        public static void Test()
        {
            lock (_lock)
            {
                SessionManager.TestSession();
            }
        }
    }

    internal class Program
    {
        static int number = 0;
        static object _obj = new object();

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();

            // 개발 단계에서는 데드락이 발생하지 않다가 실제로 서비스를 시작하면 문제가 생기는 경우가 많다
            //Thread.Sleep(100);  // 대부분 동시에 실행되는 경우는 없어서 잘 안보임

            // 보통 crash 난 다음에 고치는 경우가 많다

            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }

        static void Thread_1()
        {
            for (int i = 0; i < 10000; i++)
            {
                SessionManager.Test();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 10000; i++)
            {
                UserManager.Test();
            }
        }
    }

    // 직접 Lock을 만들어 Lock별로 id를 주고 id를 비교해서 더 높으면 crash를 내는 등의 방법을 사용할 수도 있음
    // Lock id 호출 순서를 추적하고 그래프 구조를 만들어서 그래프에 cycle이 있는지를 보면 데드락 상황을 발견할 수 있도 있음
    // 데드락을 미리 예방하는건 아니다
    // 데드락이 발생하면 해결하는게 더 쉽다
    class CustomLock
    {
        public int id;
    }

}
