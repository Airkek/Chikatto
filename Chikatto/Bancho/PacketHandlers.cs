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
            [OsuChannelJoin] = ChannelJoin,
            [OsuChannelPart] = ChannelLeave
        };

        public async static Task Logout(Packet packet, Presence user)
        {
            user.LastPong = 0;
            
            await Global.Manager.RemoveUserById(user.Id);
            
            Console.WriteLine($"{user} logged out");
        }

        public async static Task ChannelJoin(Packet packet, Presence user)
        {
            using var readable = new ReadablePacket(packet);
            var channel = readable.Reader.ReadString();
            
            user.WaitingPackets.Enqueue(FastPackets.ChannelJoinSuccess(channel));
            user.WaitingPackets.Enqueue(FastPackets.ChannelInfo(channel, "test", Global.Manager.Count));
            
            Console.WriteLine($"{user} joined {channel}");
        }
        
        public async static Task ChannelLeave(Packet packet, Presence user)
        {
            using var readable = new ReadablePacket(packet);
            var channel = readable.Reader.ReadString();
            
            user.WaitingPackets.Enqueue(FastPackets.ChannelKick(channel));
            user.WaitingPackets.Enqueue(FastPackets.ChannelInfo(channel, "test", Global.Manager.Count - 1));
            Console.WriteLine($"{user} left from {channel}");

            //TODO channels
        }
        
        public async static Task UserStatsRequest(Packet packet, Presence user)
        {
            using var p = new ReadablePacket(packet);

            var players = p.Reader.ReadInt32Array();
            
            foreach (var i in players)
            {
                if (i == 1)
                {
                    user.WaitingPackets.Enqueue(FastPackets.BotStats());
                    user.WaitingPackets.Enqueue(FastPackets.BotPresence());
                    continue;
                }

                var us = Global.Manager.GetById(i);
                if (us != null)
                {
                    user.WaitingPackets.Enqueue(await FastPackets.UserStats(us));
                    user.WaitingPackets.Enqueue(FastPackets.UserPresence(us));
                }
                else
                {
                    user.WaitingPackets.Enqueue(FastPackets.Logout(i));
                }
            }
        }
        
        public async static Task Handle(this Packet packet, Presence user)
        {
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

        private delegate Task PacketHandler(Packet packet, Presence presence);
    }
}