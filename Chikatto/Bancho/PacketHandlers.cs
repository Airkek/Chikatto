using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            [OsuPong] = async (_, _) => { },
            [OsuLogout] = Logout,
            [OsuUserStatsRequest] = UserStatsRequest,
            [OsuChannelJoin] = ChannelJoin,
            [OsuChannelPart] = ChannelLeave,
            [OsuSendPublicMessage] = SendPublicMessage,
            [OsuSendPrivateMessage] = SendPrivateMessage,
            [OsuChangeAction] = UpdateAction,
            [OsuRequestStatusUpdate] = StatsUpdate,
            [OsuJoinLobby] = LobbyJoin,
            [OsuPartLobby] = LobbyPart,
            [OsuFriendAdd] = AddFriend,
            [OsuFriendRemove] = RemoveFriend
        };
#if DEBUG
        private static readonly PacketType[] IgnoreLog =
        {
            OsuPong,
            OsuUserStatsRequest
        };
#endif

        public static async Task UpdateAction(PacketReader reader, Presence user)
        {
            user.Status = reader.ReadBanchoObject<BanchoUserStatus>();
        }
        
        private static async Task SendPublicMessage(PacketReader reader, Presence user)
        {
            var message = reader.ReadBanchoObject<BanchoMessage>();

            if (!user.JoinedChannels.ContainsKey(message.To))
            {
                await user.Notify("Вы не находитесь в данном канале!");
                return;
            }

            var channel = user.JoinedChannels[message.To];

            await channel.WriteMessage(message.Body, user);
            
            XConsole.Log($"{user} -> {message.To}: {message.Body}");
        }

        private static async Task SendPrivateMessage(PacketReader reader, Presence user)
        {
            var message = reader.ReadBanchoObject<BanchoMessage>();

            var location = Global.OnlineManager.GetByName(message.To);

            if (location is null)
            {
                await user.Notify("Пользователь не в сети.");
                return;
            }

            await location.SendMessage(message.Body, user);

            XConsole.Log($"{user} -> {location}: {message.Body}");
        }

        private static async Task StatsUpdate(PacketReader reader, Presence user)
        {
            user.WaitingPackets.Enqueue(await FastPackets.UserStats(user));
        }

        private static async Task Logout(PacketReader reader, Presence user)
        {
            user.LastPong = 0;
            
            foreach (var (_, c) in Global.Channels)
                await c.RemoveUser(user);

            await Global.OnlineManager.Remove(user);
            await Global.OnlineManager.AddPacketToAllUsers(await FastPackets.Logout(user.Id));
            
            XConsole.Log($"{user} logged out", ConsoleColor.Green);
        }

        private static async Task ChannelJoin(PacketReader reader, Presence user)
        {
            var channel = reader.ReadString();
            
            if (!Global.Channels.ContainsKey(channel))
                return;

            var c = Global.Channels[channel];

            if (c.IsLobby && !user.InLobby)
                return;
            
            await c.JoinUser(user);
            
            XConsole.Log($"{user} joined {c}", ConsoleColor.Cyan);
        }

        private static async Task ChannelLeave(PacketReader reader, Presence user)
        {
            var channel = reader.ReadString();
            
            if (!Global.Channels.ContainsKey(channel))
                return;

            var c = Global.Channels[channel];

            await c.RemoveUser(user);

            XConsole.Log($"{user} left from {channel}", ConsoleColor.Cyan);
        }

        private static async Task LobbyJoin(PacketReader reader, Presence user)
        {
            //TODO: send available multiplayer rooms
            user.InLobby = true;
        }

        private static async Task LobbyPart(PacketReader reader, Presence user)
        {
            user.InLobby = false;
        }

        private static Task AddFriend(PacketReader reader, Presence user)
        {
            var id = reader.ReadInt32();
            return user.AddFriend(id);
        }
        
        private static Task RemoveFriend(PacketReader reader, Presence user)
        {
            var id = reader.ReadInt32();
            return user.RemoveFriend(id);
        }

        private static async Task UserStatsRequest(PacketReader reader, Presence user)
        {
            var players = reader.ReadInt32Array();
            
            foreach (var i in players)
            {
                if (i == Global.Bot.Id)
                {
                    user.WaitingPackets.Enqueue(await FastPackets.BotStats());
                    continue;
                }

                var us = Global.OnlineManager.GetById(i);
                if (us is not null)
                    user.WaitingPackets.Enqueue(await FastPackets.UserStats(us));
                else
                    user.WaitingPackets.Enqueue(await FastPackets.Logout(i));
            }
        }
        
        public static async Task Handle(this Packet packet, Presence user)
        {
            if (!Handlers.ContainsKey(packet.Type))
            {
                XConsole.Log($"{user}: Unhandled packet: {packet}", ConsoleColor.Yellow);
                return;
            }
            
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            
            try
            {
                await using var reader = PacketReader.Create(packet);
                await Handlers[packet.Type].Invoke(reader, user);
                
#if DEBUG
                if(!IgnoreLog.Contains(packet.Type))
                    XConsole.Log($"{user}: Handled: {packet} (handle took {sw.Elapsed.TotalMilliseconds}ms)", back: ConsoleColor.Green);
#endif
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                XConsole.Log($"{user}: Handle failed: {packet}", back: ConsoleColor.Red);
            }
        }

        private delegate Task PacketHandler(PacketReader packet, Presence presence);
    }
}