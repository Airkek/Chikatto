using Chikatto.Bancho.Enums;

namespace Chikatto.Bancho
{
    public class Packet
    {
        public PacketType Type;
        public PacketData Data;

        public override string ToString() => $"Packet: {Type}, Data Length: {Data.Data.Length}";
    }
}