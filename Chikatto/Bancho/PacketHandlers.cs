using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho.Enums;
using Chikatto.Database.Models;
using Chikatto.Objects;
using Chikatto.Utils;
using static Chikatto.Bancho.Enums.PacketType;

namespace Chikatto.Bancho
{
    public static class PacketHandlers
    {
        private static Dictionary<PacketType, PacketHandler> Handlers { get; } = new()
        {
            [OsuPong] = async (x, y) => { },
            [OsuLogout] = Logout,
            [OsuUserStatsRequest] = UserStatsRequest,
        };

        public async static Task Logout(Packet packet, User user)
        {
            user.LastPong = 0;
            
            foreach(var item in Global.TokenCache.Where(kvp => kvp.Value == user.Id).ToList())
                Global.TokenCache.Remove(item.Key);

            Global.UserCache.Remove(user.Id);
            
            Console.WriteLine($"{user} logged out");
        }
        
        public async static Task UserStatsRequest(Packet packet, User user)
        {
            using var p = new ReadablePacket(packet);

            var players = p.Reader.ReadInt32Array();
            
            foreach (var i in players)
            {
                if (i == 1)
                {
                    user.WaitingPackets.Add(FastPackets.BotStats());
                    user.WaitingPackets.Add(FastPackets.BotPresence());
                    continue;
                }
                if (Global.UserCache.ContainsKey(i))
                {
                    user.WaitingPackets.Add(await FastPackets.UserStats(Global.UserCache[i]));
                    user.WaitingPackets.Add(FastPackets.UserPresence(Global.UserCache[i]));
                }
                else
                {
                    user.WaitingPackets.Add(FastPackets.Logout(i));
                }
            }
        }
        
        public async static Task Handle(this Packet packet, User user)
        {
            if(!Global.UserCache.ContainsKey(user.Id))
                return;
            
            user.LastPong = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
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