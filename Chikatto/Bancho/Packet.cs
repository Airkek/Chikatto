using System;
using Chikatto.Bancho.Enums;

namespace Chikatto.Bancho
{
    public class Packet 
    {
        public PacketType Type;
        public byte[] Data = Array.Empty<byte>();

        public Packet(PacketType type)
        {
            Type = type;
        }
        
        public override string ToString() => $"Packet: {Type}, Data Length: {Data.Length}";
    }
}