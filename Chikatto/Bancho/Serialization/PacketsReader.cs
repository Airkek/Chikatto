using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chikatto.Bancho.Enums;

namespace Chikatto.Bancho.Serialization
{
    public class PacketsReader
    {
        public static IEnumerable<Packet> Read(byte[] data)
        {
            var packets = new List<Packet>();
            while (data.Length >= 7)
            {
                var packetType = (PacketType)BitConverter.ToUInt16(data.Take(2).ToArray());
                data = data.Skip(3).ToArray();
                
                var length = (int)BitConverter.ToUInt32(data.Take(4).ToArray());
                data = data.Skip(4).ToArray();

                var packetData = data.Take(length).ToArray();
                data = data.Skip(length).ToArray();
                
                packets.Add(new Packet
                {
                    Type = packetType,
                    Data = new PacketData
                    {
                        Data = packetData
                    }
                });
            }

            return packets;
        }
    }
}