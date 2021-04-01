using System.Threading.Tasks;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Serialization;

namespace Chikatto.Bancho
{
    public class PacketWriter : SerializationWriter
    {
        public readonly PacketType Type;
        private readonly PacketStream _stream;
        
        public PacketWriter(PacketStream packet) : base(packet)
        {
            Type = packet.Type;
            _stream = packet;
        }

        public static PacketWriter Create(Packet packet)
        {
            return new(new PacketStream(packet));
        }
        
        public static PacketWriter Create(PacketType type)
        {
            return new(new PacketStream(type));
        }

        public Packet Dump() => _stream.Dump();

        public new void Dispose()
        {
            _stream.Dispose();
            base.Dispose();
        }

        public new async ValueTask DisposeAsync()
        {
            await _stream.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}