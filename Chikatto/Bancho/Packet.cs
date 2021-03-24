using System;
using Chikatto.Bancho.Enums;

namespace Chikatto.Bancho
{
    public class Packet
    {
        public PacketType Type;
        public byte[] Data;

        public Packet(PacketType type, byte[] data = null)
        {
            Type = type;
            Data = data ?? Array.Empty<byte>();
        }

        public override string ToString() => $"Packet: {Type}, Data Length: {Data.Length}";
    }
}