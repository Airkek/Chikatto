using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Chikatto.Bancho.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using static Chikatto.Bancho.Enums.PacketType;

namespace Chikatto.Bancho
{
    public static class PacketHandlers
    {
        private static Dictionary<PacketType, PacketHandler> Handlers { get; } = new()
        {
            [OsuPong] = async (x, y) => null, //TODO: set user last pong time
        };
        
        public async static Task<Packet> Handle(this Packet packet, string token)
        {
            if (!Handlers.ContainsKey(packet.Type))
            {
                Console.WriteLine($"NotImplementedPacket: {packet}");
                return null;
            }
            
            var sw = Stopwatch.StartNew();
            var outPacket = await Handlers[packet.Type].Invoke(packet.Data, token);
            sw.Stop();
            Console.WriteLine($"Handled: {packet} (handle took {sw.Elapsed.TotalMilliseconds}ms)");
            
            return outPacket;
        }

        private delegate Task<Packet> PacketHandler(byte[] Data, string token);
    }
}