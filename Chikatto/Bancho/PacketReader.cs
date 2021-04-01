using System;
using System.Threading.Tasks;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Serialization;

namespace Chikatto.Bancho
{
    public class PacketReader : SerializationReader, IAsyncDisposable
    {
        public readonly PacketType Type;
        private readonly PacketStream _stream;
        
        public PacketReader(PacketStream packet) : base(packet)
        {
            Type = packet.Type;
            _stream = packet;
        }

        public static PacketReader Create(Packet packet)
        {
            return new(new PacketStream(packet));
        }

        public Packet Dump() => _stream.Dump();

        public new void Dispose()
        {
            _stream.Dispose();
            base.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await _stream.DisposeAsync();
            base.Dispose(); // :C
        }
    }
}