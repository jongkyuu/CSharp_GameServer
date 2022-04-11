using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {

        static void Main(string[] args)
        {
            int[,] arr = new int[10000, 10000];

            {
                // 캐시는 지역성을 바탕으로 동작하기 때문에 특정 데이터의 물리적 공간 주변의 데이터를 미리 저장해 두기 때문에 위의 Case가 더 빠르다
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                    for (int x = 0; x < 10000; x++)
                        arr[y, x] = 1;
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"걸린시간 : {end - now}");
            }

            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                    for (int x = 0; x < 10000; x++)
                        arr[x, y] = 1;
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"걸린시간 : {end - now}");
            }
        }

    }
}
