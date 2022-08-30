using ServerCore;
using System;

class PacketHandler
{
    public static void PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {

    }
    public static void TestHandler(PacketSession session, IPacket packet)
    {
        // PDL에 packet을 추가하면 PacketHandler에 해당 Handler 함수를 추가해 줘야한다
    }
}