using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Chikatto.Bancho.Enums;

namespace Chikatto.Bancho
{
    public class PacketStream : MemoryStream
    {
        public PacketType Type;
        
        public PacketStream(PacketType type)
        {
            Type = type;
        }
        
        public PacketStream(PacketType type, byte[] data) : base(data)
        {
            Type = type;
        }

        public PacketStream(Packet packet) : base(packet.Data)
        {
            Type = packet.Type;
        }

        public Packet Dump() => new(Type, ToArray());
    }
}