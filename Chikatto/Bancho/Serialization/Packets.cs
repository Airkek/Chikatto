using System;
using System.Collections.Generic;
using System.Linq;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Serialization.Extensions;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Chikatto.Bancho.Serialization
{
    public static class Packets
    {
        public static byte[] Write(IEnumerable<Packet> packets)
        {
            var bytes = new List<byte>();

            foreach (var packet in packets)
            {
                ((ushort) packet.Type).GetBytes().ToList().ForEach(bytes.Add);
                bytes.Add(0);
                ((uint) packet.Data.Length).GetBytes().ToList().ForEach(bytes.Add);
                packet.Data.ToList().ForEach(bytes.Add);
            }

            return bytes.ToArray();
        }
        
        public static IEnumerable<Packet> Read(byte[] data)
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