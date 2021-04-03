using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;
using Chikatto.Constants;
using Chikatto.Database.Models;
using Chikatto.Multiplayer;
using Chikatto.Objects;
using Chikatto.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Internal;
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
            [OsuJoinMatch] = MatchJoin,
            [OsuMatchNotReady] = MatchNotReady,
            [OsuMatchHasBeatmap] = MatchNotReady,
            [OsuMatchReady] = MatchReady,
            [OsuMatchNoBeatmap] = MatchNoBeatmap,
            [OsuMatchSlotLock] = MatchSlotLock,
            [OsuMatchChangeSlot] = MatchChangeSlot,
            [OsuMatchTransferHost] = MatchTransferHost,
            [OsuMatchChangeTeam] = MatchChangeTeam,
            [OsuMatchChangeMods] = MatchChangeMods,
            [OsuMatchChangePassword] = MatchChangePassword,
            [OsuMatchSkipRequest] = MatchSkipRequest,
            [OsuMatchStart] = MatchStart,
            [OsuMatchLoadComplete] = MatchLoadComplete,
            [OsuMatchFailed] = MatchPlayerFail,
            [OsuMatchComplete] = MatchCompleted,
            [OsuMatchScoreUpdate] = MatchScoreUpdate
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

        public static async Task MatchJoin(PacketReader reader, Presence user)
        {
            var id = reader.ReadInt32();
            if (user.Match is not null || !Global.Rooms.ContainsKey(id))
            {
                user.WaitingPackets.Enqueue(FastPackets.MatchJoinFail);
                return;
            }

            var password = reader.ReadString();
            var match = Global.Rooms[id];

            await match.Join(user, password);
            
            XConsole.Log($"{user} joined to multiplayer room {match}", ConsoleColor.Cyan);
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
            if (user.Match is null)
                return;

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

        public static async Task MatchSlotLock(PacketReader reader, Presence user)
        {
            if(user.Match is null || user.Match.InProgress || user.Match.HostId != user.Id)
                return;

            var index = reader.ReadInt32();
            
            if(index < 0 || index > 15)
                return;

            var slot = user.Match.Slots.ElementAt(index);

            await slot.Toggle();
            await user.Match.Update();
        }

        public static async Task MatchChangePassword(PacketReader reader, Presence user)
        {
            if (user.Match is null || user.Match.HostId != user.Id)
                return;

            user.Match.Password = reader.ReadString();
            await user.Match.Update();
        }
        
        public static async Task MatchScoreUpdate(PacketReader reader, Presence user)
        {
            if (user.Match is null || !user.Match.InProgress)
                return;

            var match = user.Match;

            var bytes = reader.ReadBytes(27);
            var index = match.Slots.Select(x => x.UserId).IndexOf(user.Id);

            bytes[4] = (byte) index;

            await match.AddPacketsToSpecificPlayers(await FastPackets.MatchScoreUpdate(bytes.ToArray()));
        }

        public static async Task MatchPlayerFail(PacketReader reader, Presence user)
        {
            if (user.Match is null || !user.Match.InProgress)
                return;

            var match = user.Match;
            var index = match.Slots.Select(x => x.UserId).IndexOf(user.Id);

            await match.AddPacketsToSpecificPlayers(await FastPackets.MatchPlayerFailed(index));
        }

        public static async Task MatchCompleted(PacketReader reader, Presence user)
        {
            if (user.Match is null)
                return;
            
            var match = user.Match;
            var slot = match.GetSlot(user.Id);

            slot.Status = SlotStatus.Complete;
            
            if(match.Slots.Any(x => x.Status == SlotStatus.Playing))
                return;

            await match.Unready();
            await match.AddPacketsToAllPlayers(FastPackets.MatchComplete);
            await match.Update();
        }

        public static async Task MatchChangeMods(PacketReader reader, Presence user)
        {
            if(user.Match is null || user.Match.InProgress)
                return;

            var mods = (Mods) reader.ReadInt32();

            var match = user.Match;
            if (match.FreeMod)
            {
                if (match.HostId == user.Id)
                    match.Mods = mods & Mods.SpeedAltering;

                var slot = match.GetSlot(user.Id);
                slot.Mods = mods & ~Mods.SpeedAltering;
            }
            else if (match.HostId == user.Id)
            {
                match.Mods = mods;
            }
            else
                return;

            await match.Update();
        }
        
        public static async Task MatchChangeTeam(PacketReader reader, Presence user)
        {
            if(user.Match is null || user.Match.InProgress || 
               user.Match.TeamType != MatchTeamType.TagTeamVS && user.Match.TeamType != MatchTeamType.TeamVS)
                return;

            var slot = user.Match.GetSlot(user.Id);
            slot.Team = slot.Team == MatchTeam.Blue ? MatchTeam.Red : MatchTeam.Blue;
            await user.Match.Update();
        }

        public static async Task MatchStart(PacketReader reader, Presence user)
        {
            if(user.Match is null || user.Id != user.Match.HostId)
                return;

            await user.Match.Start();
            await user.Match.Update();
        }
        
        public static async Task MatchLoadComplete(PacketReader reader, Presence user)
        {
            if(user.Match is null)
                return;

            var match = user.Match;
            var slot = match.GetSlot(user.Id);
            
            if(slot.Status != SlotStatus.Playing)
                return;

            if (--match.NeedLoad <= 0)
            {
                await match.AddPacketsToAllPlayers(FastPackets.MatchAllPlayersLoaded);
                match.NeedLoad = 0;
            }
        }

        public static async Task MatchTransferHost(PacketReader reader, Presence user)
        {
            if(user.Match is null || user.Match.InProgress || user.Match.HostId != user.Id)
                return;

            var index = reader.ReadInt32();
            
            if(index < 0 || index > 15)
                return;

            var slot = user.Match.Slots.ElementAt(index);
            
            if((slot.Status & SlotStatus.HasPlayer) == 0)
                return;

            user.Match.Host = slot.User;
            user.WaitingPackets.Enqueue(FastPackets.MatchTransferHost);
            await user.Match.Update();
        }

        public static async Task MatchSkipRequest(PacketReader reader, Presence user)
        {
            if(user.Match is null)
                return;

            var match = user.Match;

            var uSlot = match.GetSlot(user.Id);
            uSlot.Skipped = true;
            
            if (match.Slots.All(slot => slot.Status == SlotStatus.Playing && slot.Skipped))
                await match.AddPacketsToAllPlayers(FastPackets.MatchSkip);
        }
        
        public static async Task MatchChangeSlot(PacketReader reader, Presence user)
        {
            if(user.Match is null || user.Match.InProgress)
                return;

            var index = reader.ReadInt32();
            
            if(index < 0 || index > 15)
                return;

            var slot = user.Match.Slots.ElementAt(index);
            
            if(slot.Status == SlotStatus.Locked || (slot.Status & SlotStatus.HasPlayer) != 0)
                return;
            
            var uSlot = user.Match.GetSlot(user.Id);

            slot.Status = uSlot.Status;
            slot.Mods = uSlot.Mods;
            slot.User = uSlot.User;
            slot.Team = uSlot.Team;
            
            uSlot.User = null;
            uSlot.Mods = Mods.NoMod;
            uSlot.Team = MatchTeam.Neutral;
            uSlot.Status = SlotStatus.Open;
            
            await user.Match.Update();
        }

        public static async Task MatchReady(PacketReader reader, Presence user)
        {
            if(user.Match is null)
                return;
            
            await user.Match.UpdateUserStatus(user, SlotStatus.Ready);
        }
        
        public static async Task MatchNotReady(PacketReader reader, Presence user)
        {
            if(user.Match is null)
                return;
            
            await user.Match.UpdateUserStatus(user, SlotStatus.NotReady);
        }
        
        public static async Task MatchNoBeatmap(PacketReader reader, Presence user)
        {
            if(user.Match is null)
                return;
            
            await user.Match.UpdateUserStatus(user, SlotStatus.NoMap);
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

            if (user.Match is not null)
                await user.Match.Leave(user);
            
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