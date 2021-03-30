using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;
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
            [OsuChannelPart] = ChannelLeave,
            [OsuSendPublicMessage] = SendPublicMessage,
            [OsuSendPrivateMessage] = SendPrivateMessage,
            [OsuChangeAction] = ActionUpdate,
            [OsuRequestStatusUpdate] = StatsUpdate
        };
#if DEBUG
        private static PacketType[] IgnoreLog =
        {
            OsuPong,
            OsuUserStatsRequest
        };
#endif

        public async static Task SendPublicMessage(Packet packet, Presence user)
        {
            using var readable = new ReadablePacket(packet);
            var message = readable.Reader.ReadBanchoObject<BanchoMessage>();

            if (!Global.Channels.ContainsKey(message.To))
                return;

            message.From = user.Name;

            var c = Global.Channels[message.To];
            c.WriteMessage(user, message);
            
            Console.WriteLine($"{user} -> {message.To}: {message.Body}");
        }

        public async static Task SendPrivateMessage(Packet packet, Presence user)
        {
            using var readable = new ReadablePacket(packet);
            var message = readable.Reader.ReadBanchoObject<BanchoMessage>();

            var location = Global.OnlineManager.GetByName(message.To);

            if (location == null)
            {
                message.To = message.From;
                message.From = Global.Bot.Name;
                message.Body = "Пользователь не в сети, дружище";
                
                user.WaitingPackets.Enqueue(FastPackets.SendMessage(message));
                
                return;
            }
            
            location.WaitingPackets.Enqueue(FastPackets.SendMessage(message));

            Console.WriteLine($"{user} -> {message.To}: {message.Body}");
        }

        public static async Task ActionUpdate(Packet packet, Presence user)
        {
            using var readable = new ReadablePacket(packet);

            user.Status = readable.Reader.ReadBanchoObject<BanchoUserStatus>();
        }

        public static async Task StatsUpdate(Packet packet, Presence user)
        {
            user.WaitingPackets.Enqueue(await FastPackets.UserStats(user));
        }

        public async static Task Logout(Packet packet, Presence user)
        {
            //TODO: Presence.Logout()
            user.LastPong = 0;
            
            foreach (var (_, c) in Global.Channels)
                c.RemoveUser(user);

            await Global.OnlineManager.RemoveUserById(user.Id);
            
            await Global.OnlineManager.AddPacketToAllUsers(FastPackets.Logout(user.Id));
            
            Console.WriteLine($"{user} logged out");
        }

        public async static Task ChannelJoin(Packet packet, Presence user)
        {
            using var readable = new ReadablePacket(packet);
            var channel = readable.Reader.ReadString();
            
            if (!Global.Channels.ContainsKey(channel))
                return;

            var c = Global.Channels[channel];
            c.JoinUser(user);
            
            Console.WriteLine($"{user} joined {c}");
        }
        
        public async static Task ChannelLeave(Packet packet, Presence user)
        {
            using var readable = new ReadablePacket(packet);
            var channel = readable.Reader.ReadString();
            
            if (!Global.Channels.ContainsKey(channel))
                return;

            var c = Global.Channels[channel];
            c.RemoveUser(user);
            
            Console.WriteLine($"{user} left from {channel}");

            //TODO channel leave
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

                var us = Global.OnlineManager.GetById(i);
                if (us != null)
                {
                    user.WaitingPackets.Enqueue(await FastPackets.UserStats(us));
                    user.WaitingPackets.Enqueue(await FastPackets.UserPresence(us));
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
            if(!IgnoreLog.Contains(packet.Type))
                Console.WriteLine($"{user}: Handled: {packet} (handle took {sw.Elapsed.TotalMilliseconds}ms)");
#endif
        }

        private delegate Task PacketHandler(Packet packet, Presence presence);
    }
}