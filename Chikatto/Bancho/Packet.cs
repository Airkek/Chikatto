using System;
using Chikatto.Bancho.Enums;

namespace Chikatto.Bancho
{
    public class Packet 
    {
        public readonly PacketType Type;
        public byte[] Data;

        public Packet(PacketType type)
        {
            Type = type;
            Data = Array.Empty<byte>();
        }
        
        public Packet(PacketType type, byte[] data)
        {
            Type = type;
            Data = data;
        }
        
        public override string ToString() => $"<{Type} ({Data.Length})>";
    }
}