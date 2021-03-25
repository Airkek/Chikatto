using System;
using System.IO;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Serialization;

namespace Chikatto.Bancho
{
    public class WriteablePacket : StreamPacket
    {
        public readonly SerializationWriter Writer;

        public WriteablePacket(PacketType type) : base(type)
        {
            Writer = new SerializationWriter(Stream);
        }

        public new void Dispose()
        {
            Writer.Dispose();
            base.Dispose();
        }
    }
}