using System;
using System.Collections.Generic;

namespace Server
{
    class GameRoom
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        object _lock = new object();

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId;
            packet.chat = chat;
            ArraySegment<byte> segment = packet.Write();

            // 공유하고 있는 변수 _sessions에 접근하므로 lock을 걸어준다
            lock(_lock)
            {
                foreach(ClientSession s in _sessions)
                {
                    s.Send(segment);
                }
            }
        }

        public void Enter(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Add(session);
                session.Room = this;
            }
        }

        public void Leave(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session);
            }
        }
    }
}
