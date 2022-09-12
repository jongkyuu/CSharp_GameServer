#!/bin/bash
../../PacketGenerator/bin/Debug/net6.0/PacketGenerator ../../PacketGenerator/PDL.xml
cp GenPackets.cs "../../DummyClient/Packet"
cp GenPackets.cs "../../Server/Packet"
cp ClientPacketManager.cs "../../DummyClient/Packet"
cp ServerPacketManager.cs "../../Server/Packet"