using DummyClient;
using ServerCore;
using System;

class PacketHandler
{
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        // PDL에 packet을 추가하면 PacketHandler에 해당 Handler 함수를 추가해 줘야한다
        S_Chat chatPacket = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;

        //if (chatPacket.playerId == 1)
            Console.WriteLine(chatPacket.chat);
    }
}