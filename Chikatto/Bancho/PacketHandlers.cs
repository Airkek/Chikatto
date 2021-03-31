using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;
using Chikatto.Objects;
using Chikatto.Utils;
using static Chikatto.Bancho.Enums.PacketType;

namespace Chikatto.Bancho
{
    public static class PacketHandlers
    {
        private static readonly Dictionary<PacketType, PacketHandler> Handlers = new()
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
        private static readonly PacketType[] IgnoreLog =
        {
            OsuPong,
            OsuUserStatsRequest
        };
#endif

        private static async Task SendPublicMessage(Packet packet, Presence user)
        {
            await using var readable = new ReadablePacket(packet);
            var message = readable.Reader.ReadBanchoObject<BanchoMessage>();

            if (!Global.Channels.ContainsKey(message.To))
                return;

            message.From = user.Name;

            var c = Global.Channels[message.To];
            await c.WriteMessage(user, message);
            
            XConsole.Log($"{user} -> {message.To}: {message.Body}");
        }

        private static async Task SendPrivateMessage(Packet packet, Presence user)
        {
            await using var readable = new ReadablePacket(packet);
            var message = readable.Reader.ReadBanchoObject<BanchoMessage>();

            var location = Global.OnlineManager.GetByName(message.To);

            if (location is null)
            {
                message.To = message.From;
                message.From = Global.Bot.Name;
                message.Body = "Пользователь не в сети, дружище";
                
                user.WaitingPackets.Enqueue(await FastPackets.SendMessage(message));
                
                return;
            }
            
            location.WaitingPackets.Enqueue(await FastPackets.SendMessage(message));

            XConsole.Log($"{user} -> {message.To}: {message.Body}");
        }

        private static async Task ActionUpdate(Packet packet, Presence user)
        {
            await using var readable = new ReadablePacket(packet);

            user.Status = readable.Reader.ReadBanchoObject<BanchoUserStatus>();
        }

        private static async Task StatsUpdate(Packet packet, Presence user)
        {
            user.WaitingPackets.Enqueue(await FastPackets.UserStats(user));
        }

        private static async Task Logout(Packet packet, Presence user)
        {
            //TODO: Presence.Logout()
            user.LastPong = 0;
            
            foreach (var (_, c) in Global.Channels)
                await c.RemoveUser(user);

            await Global.OnlineManager.Remove(user);
            await Global.OnlineManager.AddPacketToAllUsers(await FastPackets.Logout(user.Id));
            
            XConsole.Log($"{user} logged out", ConsoleColor.Green);
        }

        private static async Task ChannelJoin(Packet packet, Presence user)
        {
            await using var readable = new ReadablePacket(packet);
            var channel = readable.Reader.ReadString();
            
            if (!Global.Channels.ContainsKey(channel))
                return;

            var c = Global.Channels[channel];
            await c.JoinUser(user);
            
            XConsole.Log($"{user} joined {c}", ConsoleColor.Cyan);
        }

        private static async Task ChannelLeave(Packet packet, Presence user)
        {
            await using var readable = new ReadablePacket(packet);
            var channel = readable.Reader.ReadString();
            
            if (!Global.Channels.ContainsKey(channel))
                return;

            var c = Global.Channels[channel];
            await c.RemoveUser(user);

            XConsole.Log($"{user} left from {channel}", ConsoleColor.Cyan);
        }

        private static async Task UserStatsRequest(Packet packet, Presence user)
        {
            await using var p = new ReadablePacket(packet);

            var players = p.Reader.ReadInt32Array();
            
            foreach (var i in players)
            {
                if (i == 1)
                {
                    user.WaitingPackets.Enqueue(await FastPackets.BotStats());
                    user.WaitingPackets.Enqueue(await FastPackets.BotPresence());
                    continue;
                }

                var us = Global.OnlineManager.GetById(i);
                if (us is not null)
                {
                    user.WaitingPackets.Enqueue(await FastPackets.UserStats(us));
                    user.WaitingPackets.Enqueue(await FastPackets.UserPresence(us));
                }
                else
                {
                    user.WaitingPackets.Enqueue(await FastPackets.Logout(i));
                }
            }
        }
        
        public static async Task Handle(this Packet packet, Presence user)
        {
            if (!Handlers.ContainsKey(packet.Type))
            {
#if DEBUG
                XConsole.Log($"{user}: NotImplementedPacket: {packet}", back: ConsoleColor.Red);
#endif
                
                return;
            }
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            try
            {
                await Handlers[packet.Type].Invoke(packet, user);
#if DEBUG
                if(!IgnoreLog.Contains(packet.Type))
                    XConsole.Log($"{user}: Handled: {packet} (handle took {sw.Elapsed.TotalMilliseconds}ms)", back: ConsoleColor.Green);
#endif
            }
            catch
            {
                XConsole.Log($"{user}: Handle failed: {packet}", back: ConsoleColor.Red);
            }
        }

        private delegate Task PacketHandler(Packet packet, Presence presence);
    }
}