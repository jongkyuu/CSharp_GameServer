using System;
using System.Threading;

namespace ContextSwitchingDemo
{
    class Memo
    {
        public void Test()
        {
            Thread.Sleep(1);  // 무조건 휴식 -> 1ms 정도 쉰다고 요청. 운영체제 스케쥴러가 요청한 시간과 비슷하게 쉬도록 맞춤
            Thread.Sleep(0);  // 조건부 양보 -> 나보다 우선순위가 낮은 애들한테는 양보 불가. 우선순위가 나보다 같거나 높은 쓰레드가 없으면 다시 본인한테 순서가 돌아옴
            Thread.Yield();  // 관대한 양보 -> 지금 실행이 가능한 쓰레드가 있으면 실행하도록 함. 실행 가능한 쓰레드가 없으면 남은 시간을 본인에게 소진

            // 만드는 프로그램에 따라 가장 좋은 Case가 조금씩 달라질 수도 있다

            // Context Switching : 현재 진행하고 있는 Task(Process, Thread)의 상태를 저장하고 다음 진행할 Task의 상태 값을 읽어 적용하는 과정을 말함
            // 참고 : https://nesoy.github.io/articles/2018-11/Context-Switching
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
