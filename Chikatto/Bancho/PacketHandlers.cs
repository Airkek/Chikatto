using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;
using Chikatto.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using static Chikatto.Bancho.Enums.PacketType;

namespace Chikatto.Bancho
{
    public static class PacketHandlers
    {
        private static Dictionary<PacketType, PacketHandler> Handlers { get; } = new()
        {
            [OsuPong] = async (x, y) => { }, //TODO: set user last pong time
            [OsuLogout] = async (x, user) => Global.UserCache.Remove(user.Id),
            
        };
        
        public async static Task Handle(this Packet packet, User user)
        {
            if (!Handlers.ContainsKey(packet.Type))
            {
                Console.WriteLine($"NotImplementedPacket: {packet}");
                return;
            }
            
            var sw = Stopwatch.StartNew();
            await Handlers[packet.Type].Invoke(packet.Data, user);
            sw.Stop();
            Console.WriteLine($"Handled: {packet} (handle took {sw.Elapsed.TotalMilliseconds}ms)");
        }

        private delegate Task PacketHandler(byte[] Data, User token);
    }
}