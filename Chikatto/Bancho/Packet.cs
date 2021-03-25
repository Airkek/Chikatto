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
        
        public Packet(PacketType type, byte[] data)
        {
            Type = type;
            Data = data;
        }
        
        public override string ToString() => $"<{Type} ({Data.Length})>";
    }
}