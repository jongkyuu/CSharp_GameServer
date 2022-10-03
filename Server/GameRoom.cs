using ServerCore;
using System;
using System.Collections.Generic;

namespace Server
{
    class GameRoom : IJobQueue
    {
        // _sessions은 공유하고 있는 변수
        List<ClientSession> _sessions = new List<ClientSession>();
        object _lock = new object();
        
        JobQueue _jobQueue = new JobQueue();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId;
            packet.chat = $"{chat}, I'm {packet.playerId}";
            ArraySegment<byte> segment = packet.Write();

            // 공유하고 있는 변수 _sessions에 접근하므로 lock을 걸어줬음
            // _jobQueue에서 Pop을 할때 lock을 걸어주어 쓰레드 하나만 실행할 수 있게 보장
            foreach(ClientSession s in _sessions)
            {
                s.Send(segment);
            }
        }

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);
        }
    }
}
