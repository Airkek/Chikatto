using System;
using System.IO;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Serialization;

namespace Chikatto.Bancho
{
    public class StreamPacket : Packet, IDisposable
    {
        public readonly MemoryStream Stream;
        public new byte[] Data => Stream.ToArray();

        public StreamPacket(PacketType type) : base(type)
        {
            Stream = new MemoryStream();
        }

        public Packet Dump() => new(Type) { Data = Data };

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}