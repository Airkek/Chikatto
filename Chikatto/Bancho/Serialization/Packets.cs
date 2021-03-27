using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chikatto.Bancho.Enums;
using Newtonsoft.Json.Serialization;

namespace Chikatto.Bancho.Serialization
{
    public static class Packets
    {
        public static byte[] GetBytes(IEnumerable<Packet> packets)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            foreach (var packet in packets)
            {
                writer.Write((ushort) packet.Type);
                writer.Write((byte) 0);
                writer.Write(packet.Data.Length);
                writer.Write(packet.Data);
            }

            return stream.ToArray();
        }
        
        public static IEnumerable<Packet> GetPackets(byte[] data)
        {
            var packets = new List<Packet>();
            
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);
            
            while (stream.Length - stream.Position >= 7)
            {
                var packetType = (PacketType) reader.ReadByte();
                
                reader.ReadByte(); // pad byte
                
                var length = reader.ReadInt32();
                var packet = new Packet(packetType);

                if (length != 0)
                    packet.Data = reader.ReadBytes(length);

                packets.Add(packet);
            }

            return packets;
        }
    }
}