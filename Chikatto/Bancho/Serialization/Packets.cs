using System;
using System.Collections.Generic;
using System.Linq;
using Chikatto.Bancho.Enums;

namespace Chikatto.Bancho.Serialization
{
    public static class Packets
    {
        public static byte[] GetBytes(IEnumerable<Packet> packets)
        {
            var bytes = new List<byte>();

            foreach (var packet in packets)
            {
                BitConverter.GetBytes((ushort) packet.Type).ToList().ForEach(bytes.Add); 
                bytes.Add(0);
                BitConverter.GetBytes((uint) packet.Data.Length).ToList().ForEach(bytes.Add);
                packet.Data.ToList().ForEach(bytes.Add);
            }

            return bytes.ToArray();
        }
        
        public static IEnumerable<Packet> GetPackets(byte[] data)
        {
            var packets = new List<Packet>();
            while (data.Length >= 7)
            {
                var packetType = (PacketType)BitConverter.ToUInt16(data.Take(2).ToArray());
                data = data.Skip(3).ToArray();
                
                var length = (int)BitConverter.ToUInt32(data.Take(4).ToArray());
                data = data.Skip(4).ToArray();

                var packet = new Packet(packetType);

                if (length != 0)
                {
                    packet.Data = data.Take(length).ToArray();
                    data = data.Skip(length).ToArray();
                }
                
                packets.Add(packet);
            }

            return packets;
        }
    }
}