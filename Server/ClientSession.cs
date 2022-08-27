using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
	class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            // 보내는 순간에 외부에서 bytes를 만들어 줬다
            // 지금은 간단하게 문자열을 byte 배열로 만들었는데
            // 실제 게임에서는 패킷이라는 정보를 만들어서 보냄

            //// TCP는 패킷이 잘려서 올 수 있다
            //Packet packet = new Packet() { size = 100, packetId = 10 };

            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);

            // 100명의 유저가 움직인다면 Send를 하는 횟수가 빈번해짐
            // sendBuff는 세션 내부보다는 외부에서 만들어서 보내주는게 효율적이다
            //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server");
            //Send(sendBuff);

            Thread.Sleep(5000);
            Disconnect();
        }
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;
            

            switch ((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        PlayerInfoReq p = new PlayerInfoReq();
                        p.Read(buffer);
                        Console.WriteLine($"PlayerInfoReq : {p.playerId} {p.name}");

                        foreach (PlayerInfoReq.Skill skill in p.skills)
                        {
							Console.WriteLine($"Skill({skill.id})({skill.level})({skill.duration})");
                        }
                    }
                    break;
            }

            Console.WriteLine($"RecvPacketId : {id}, Size : {size}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }


        // 이후에는 문자열이 아니라 약속된 프로토콜 대로 데이터를 주고 받는다
        // 이동 패킷 (3,2) 좌표로 이동하고 싶다!
        // 15 3 2
        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred Bytes : {numOfBytes}");
        }
    }

}
