using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class PacketHandler
{
    public static void PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        PlayerInfoReq p = packet as PlayerInfoReq;
        Console.WriteLine($"PlayerInfoReq : {p.playerId} {p.name}");

        foreach (PlayerInfoReq.Skill skill in p.skills)
        {
            Console.WriteLine($"Skill({skill.id})({skill.level})({skill.duration})");
        }
    }
    public static void TestHandler(PacketSession session, IPacket packet)
    {
        // PDL에 packet을 추가하면 PacketHandler에 해당 Handler 함수를 추가해 줘야한다
    }
}
