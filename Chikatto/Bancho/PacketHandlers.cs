using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Serialization;
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
            [OsuPong] = async (x, y) => { },
            [OsuLogout] = Logout,
            [OsuUserStatsRequest] = UserStatsRequest
        };

        public async static Task Logout(Packet packet, User user)
        {
            var arg = BitConverter.ToInt32(packet.Data);
            if (arg == 0)
            {
                Global.TokenCache.Remove(user.BanchoToken);
                Global.UserCache.Remove(user.Id);
                Console.WriteLine($"{user} logged out");
            }
        }
        
        public async static Task UserStatsRequest(Packet packet, User user)
        {
            using var p = new ReadablePacket(packet);

            var players = p.Reader.ReadInt32Array();
            
            foreach (var i in players)
            {
                if (Global.UserCache.ContainsKey(i))
                {
                    user.WaitingPackets.Add(FastPackets.UserStats(Global.UserCache[i]));
                    user.WaitingPackets.Add(FastPackets.UserPresence(Global.UserCache[i]));
                }
            }
        }
        
        public async static Task Handle(this Packet packet, User user)
        {
            user.LastPong = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            if (!Handlers.ContainsKey(packet.Type))
            {
#if DEBUG
                Console.WriteLine($"{user}: NotImplementedPacket: {packet}");
#endif
                
                return;
            }
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            
            await Handlers[packet.Type].Invoke(packet, user);
            
#if DEBUG
            sw.Stop();
            Console.WriteLine($"{user}: Handled: {packet} (handle took {sw.Elapsed.TotalMilliseconds}ms)");
#endif
        }

        private delegate Task PacketHandler(Packet packet, User token);
    }
}