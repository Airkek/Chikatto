using System.Collections.Generic;
using System.IO;

namespace Chikatto.Bancho.Serialization
{
    public static class Packets
    {
        public static byte[] GetBytes(IEnumerable<Packet> packets)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            foreach (var packet in packets)
                packet.WriteToStream(writer);

            return stream.ToArray();
        }
        
        public static IEnumerable<Packet> GetPackets(byte[] data)
        {
            var packets = new List<Packet>();
            
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);
            
            while (stream.Length - stream.Position >= 7)
                packets.Add(Packet.FromStream(reader));

            return packets;
        }
    }
}