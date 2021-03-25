using System;
using System.IO;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Serialization;

namespace Chikatto.Bancho
{
    public class WriteablePacket : Packet, IDisposable
    {
        public readonly MemoryStream Stream;
        public readonly SerializationWriter Writer;
        public new byte[] Data => Stream.ToArray();

        public WriteablePacket(PacketType type) : base(type)
        {
            Stream = new MemoryStream();
            Writer = new SerializationWriter(Stream);
        }

        public Packet Dump() => new(Type) { Data = Data };

        public void Dispose()
        {
            Writer.Dispose();
            Stream.Dispose();
        }
    }
}