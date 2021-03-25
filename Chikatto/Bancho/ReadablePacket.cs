using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Serialization;

namespace Chikatto.Bancho
{
    public class ReadablePacket : StreamPacket
    {
        public readonly SerializationReader Reader;

        public ReadablePacket(PacketType type) : base(type)
        {
            Reader = new SerializationReader(Stream);
        }

        public ReadablePacket(Packet packet) : base(packet.Type, packet.Data)
        {
            Reader = new SerializationReader(Stream);
        }

        public new void Dispose()
        {
            Reader.Dispose();
            base.Dispose();
        }
    }
}