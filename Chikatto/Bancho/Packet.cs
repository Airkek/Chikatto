﻿using Chikatto.Bancho.Enums;

namespace Chikatto.Bancho
{
    public class Packet
    {
        public PacketType Type;
        public byte[] Data;

        public override string ToString() => $"Packet: {Type}, Data Length: {Data.Length}";
    }
}