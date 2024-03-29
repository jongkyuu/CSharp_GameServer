﻿using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DummyClient
{
	class ServerSession : PacketSession
    {
        // TryWriteBytes 대신 이렇게도 구현할 수 있음 
        //static unsafe void ToBytes(byte[] array, int offset, ulong value)
        //{
        //    fixed (byte* ptr = &array[offset])
        //        *(ulong*)ptr = value;
        //}

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

   //         C_PlayerInfoReq packet = new C_PlayerInfoReq() { playerId = 1001, name="ABCD"};
			
			//var skill = new C_PlayerInfoReq.Skill() {id = 101, level = 1, duration = 2.0f};
			//skill.attributes.Add(new C_PlayerInfoReq.Skill.Attribute() { att = 77 });
			//packet.skills.Add(skill);

			//packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 101, level = 1, duration = 3.0f});
   //         packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 201, level = 2, duration = 4.0f});
   //         packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 301, level = 3, duration = 5.0f});
   //         packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 401, level = 4, duration = 6.0f});

   //         {
   //             ArraySegment<byte> s = packet.Write();
   //             if(s != null)
   //                 Send(s);
   //         }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);

            #region 변경 전 코드
            //string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);  // args.BytesTransferred : 몇 byte를 받았는지
            //Console.WriteLine($"[From Server] : {recvData}");
            //return buffer.Count;
            #endregion
        }

        public override void OnSend(int numOfBytes)
        {
            // 세션이 많아지면 자주 호출되기 때문에 주석처리
            //Console.WriteLine($"Transferred Bytes : {numOfBytes}");
        }
    }
}
