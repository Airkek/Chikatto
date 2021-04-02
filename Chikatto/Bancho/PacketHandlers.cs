using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;
using Chikatto.Constants;
using Chikatto.Multiplayer;
using Chikatto.Objects;
using Chikatto.Utils;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
            [OsuRequestStatusUpdate] = StatusUpdate,
            [OsuJoinLobby] = LobbyJoin,
            [OsuPartLobby] = LobbyPart,
            [OsuFriendAdd] = AddFriend,
            [OsuFriendRemove] = RemoveFriend,
            [OsuCreateMatch] = CreateMatch,
            [OsuMatchChangeSettings] = UpdateMatch,
            [OsuPartMatch] = PartMatch,
            
        };
#if DEBUG
        private static readonly PacketType[] IgnoreLog =
        {
            OsuPong,
            OsuUserStatsRequest
        };
#endif

        public static async Task CreateMatch(PacketReader reader, Presence user)
        {
            if (user.Match is not null)
            {
                user.WaitingPackets.Enqueue(FastPackets.MatchJoinFail);
                return;
            }
            
            var match = reader.ReadBanchoObject<Match>();
            match.Id = ++Global.MatchId;

            Global.Rooms[match.Id] = match;

            await match.Join(user, match.Password);
            await match.Update();

            await Global.OnlineManager.AddPacketToAllUsers(await FastPackets.NewMatch(match.Foreign()));

            XConsole.Log($"{user} created multiplayer room {match}", ConsoleColor.Green);
        }

        public static async Task PartMatch(PacketReader reader, Presence user)
        {
            var match = user.Match;
            if(match is null)
                return;

            await match.Leave(user);
            
            XConsole.Log($"{user} left from multiplayer room {match}", ConsoleColor.Cyan);
        }

        public static async Task UpdateMatch(PacketReader reader, Presence user)
        {
            var match = user.Match;
            var newMatch = reader.ReadBanchoObject<BanchoMatch>();

            if (match.BeatmapHash != newMatch.BeatmapHash ||
                match.Mode != newMatch.Mode ||
                match.Type != newMatch.Type ||
                match.ScoringType != newMatch.ScoringType ||
                match.TeamType != newMatch.TeamType)
            {
                await match.Unready();
            }

            match.Beatmap = newMatch.Beatmap;
            match.BeatmapId = newMatch.BeatmapId;
            match.BeatmapHash = newMatch.BeatmapHash;
            match.Name = newMatch.Name.Length > 0 ? newMatch.Name : $"{match.Host.Name}'s game";
            match.TeamType = newMatch.TeamType;
            match.ScoringType = newMatch.ScoringType;
            match.Type = newMatch.Type;
            match.Mode = newMatch.Mode;
            match.Seed = newMatch.Seed;
            match.FreeMod = newMatch.FreeMod;

            if (match.TeamType != newMatch.TeamType)
            {
                switch (newMatch.TeamType)
                {
                    case MatchTeamType.TagTeamVS:
                    case MatchTeamType.TeamVS:
                    {
                        for (var i = 0; i < match.Slots.Count; i++)
                        {
                            var slot = match.Slots.ElementAt(i);
                            slot.Team = i % 2 == 0 ? MatchTeam.Red : MatchTeam.Blue;
                        }
                        break;
                    }

                    default:
                    {
                        foreach (var slot in match.Slots)
                            slot.Team = MatchTeam.Neutral;
                        break;
                    }
                }
            }

            await match.Update();
        }

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

        private static async Task StatusUpdate(PacketReader reader, Presence user)
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
            
            if (!Global.Channels.ContainsKey(channel) || channel == "#lobby")
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

            if (!Global.Channels.ContainsKey(channel) || channel == "#lobby")
                return;

            var c = Global.Channels[channel];

            await c.RemoveUser(user);

            XConsole.Log($"{user} left from {channel}", ConsoleColor.Cyan);
        }

        private static async Task LobbyJoin(PacketReader reader, Presence user)
        {
            user.InLobby = true;
            await Global.Channels["#lobby"].JoinUser(user);
            
            foreach (var (_, match) in Global.Rooms)
                user.WaitingPackets.Enqueue(await FastPackets.NewMatch(match.Foreign()));

            XConsole.Log($"{user} joined to lobby", ConsoleColor.Cyan);
        }

        private static async Task LobbyPart(PacketReader reader, Presence user)
        {
            user.InLobby = false;
            await Global.Channels["#lobby"].RemoveUser(user);
            
            XConsole.Log($"{user} left from lobby", ConsoleColor.Cyan);
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
                XConsole.Log($"{user}: Not implemented packet: {packet}", back: ConsoleColor.Yellow);
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