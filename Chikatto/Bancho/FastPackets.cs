using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;
using Chikatto.Constants;
using Chikatto.Multiplayer;
using Chikatto.Objects;

namespace Chikatto.Bancho
{
    public static class FastPackets
    {
        //5
        public static Task<Packet> UserId(int id) => 
            GetPacket (PacketType.BanchoUserId, id);
        
        //7
        public static Task<Packet> SendMessage(BanchoMessage message) =>
            GetPacket(PacketType.BanchoSendMessage, message);

        //8
        public static readonly Packet Ping = new(PacketType.BanchoPing);
        
        //9
        public static Task<Packet> ChangeUsername(string oldName, string newName) =>
            GetPacket(PacketType.BanchoIrcChangeUsername, $"{oldName}>>>>{newName}");

        //11
        public static Task<Packet> UserStats(BanchoUserStats stats) =>
            GetPacket(PacketType.BanchoUserStats, stats);

        //11 overload
        public static async Task<Packet> UserStats(Presence user) => 
            await UserStats(await user.GetStats());

        //11 bot
        public static Task<Packet> BotStats()
        {
            var action = new BanchoUserStatus
            {
                Action = BanchoAction.Editing,
                Text = "Chikatto's source code",
                MapMd5 = null,
                Mods = Mods.NoMod,
                Mode = GameMode.Standard,
                MapId = 0
            };
            
            var stats = new BanchoUserStats
            {
                Id = Global.Bot.Id,
                Status = action,
                RankedScore = 0,
                Accuracy = 0,
                PlayCount = 0,
                TotalScore = 0,
                Rank = 0,
                PP = 0,
            };
            
            return UserStats(stats);
        }
        
        //12
        public static async Task<Packet> Logout(int uid) 
        {
            await using var packet = PacketWriter.Create(PacketType.BanchoUserLogout);
            packet.Write(uid);
            packet.Write((byte) 0);
            return packet.Dump();
        }
        
        //14
        public static Task<Packet> SpectatorJoined(int id) =>
            GetPacket(PacketType.BanchoSpectatorJoined, id);
        
        //14
        public static Task<Packet> SpectatorLeft(int id) =>
            GetPacket(PacketType.BanchoSpectatorLeft, id);
        
        //15
        public static async Task<Packet> SpectateFrames(byte[] frames) => 
            new(PacketType.BanchoSpectateFrames, frames);
        
        //19
        public static Packet VersionUpdate = new(PacketType.BanchoVersionUpdate); 
        
        //22
        public static Task<Packet> CantSpectate(int id) => 
            GetPacket(PacketType.BanchoCantSpectate, id);
        
        //23
        public static readonly Packet GetAttention = new(PacketType.BanchoGetAttention);

        //24
        public static Task<Packet> Notification(string text) => 
            GetPacket(PacketType.BanchoNotification, text);

        //24
        public static Task<Packet> NewMatch(BanchoMatch banchoMatch) =>
            GetPacket(PacketType.BanchoNewMatch, banchoMatch);

        //26
        public static Task<Packet> UpdateMatch(BanchoMatch banchoMatch) =>
            GetPacket(PacketType.BanchoUpdateMatch, banchoMatch);
        //26 overload
        public static Task<Packet> UpdateMatch(Match banchoMatch) =>
            GetPacket(PacketType.BanchoUpdateMatch, banchoMatch);

        //28
        public static Task<Packet> DisposeMatch(int id) =>
            GetPacket(PacketType.BanchoDisposeMatch, id);
        
        //34
        public static readonly Packet ToggleBlockNonFriendPm = new(PacketType.BanchoToggleBlockNonFriendPm);
        
        //36
        public static Task<Packet> MatchJoinSuccess(BanchoMatch banchoMatch) =>
            GetPacket(PacketType.BanchoMatchJoinSuccess, banchoMatch);

        //37
        public static readonly Packet MatchJoinFail = new(PacketType.BanchoMatchJoinFail);
        
        //42
        public static Task<Packet> FellowSpectatorJoined(int id) =>
            GetPacket(PacketType.BanchoFellowSpectatorJoined, id);
        
        //43
        public static Task<Packet> FellowSpectatorLeft(int id) =>
            GetPacket(PacketType.BanchoFellowSpectatorLeft, id);

        //46
        public static Task<Packet> MatchStart(BanchoMatch banchoMatch) =>
            GetPacket(PacketType.BanchoMatchStart, banchoMatch);

        //48
        public static async Task<Packet> MatchScoreUpdate(byte[] ScoreFrame) =>
            new(PacketType.BanchoMatchScoreUpdate, ScoreFrame);
        
        //50
        public static readonly Packet MatchTransferHost = new(PacketType.BanchoMatchTransferHost);
        
        //53
        public static readonly Packet MatchAllPlayersLoaded = new(PacketType.BanchoMatchAllPlayersLoaded);
        
        //57
        public static Task<Packet> MatchPlayerFailed(int slotId) =>
            GetPacket(PacketType.BanchoMatchPlayerFailed, slotId);

        //58
        public static readonly Packet MatchComplete = new(PacketType.BanchoMatchComplete);
        
        //61
        public static readonly Packet MatchSkip = new(PacketType.BanchoMatchSkip);

        //64
        public static Task<Packet> ChannelJoinSuccess(string channelName) =>
            GetPacket(PacketType.BanchoChannelJoinSuccess, channelName);

        //65
        public static async Task<Packet> ChannelInfo(Channel c)
        {
            await using var packet = PacketWriter.Create(PacketType.BanchoChannelInfo);
            packet.Write(c.Name);
            packet.Write(c.Topic);
            packet.Write(c.Users.Count);
            return packet.Dump();
        }
        
        //66
        public static Task<Packet> ChannelKick(string channelName) =>
            GetPacket(PacketType.BanchoChannelKick, channelName);

        //67
        public static async Task<Packet> ChannelAutoJoin(Channel c)
        {
            await using var packet = PacketWriter.Create(PacketType.BanchoChannelAutoJoin);
            packet.Write(c.Name);
            packet.Write(c.Topic);
            packet.Write(c.Users.Count);
            return packet.Dump();
        }
        
        //69 unused

        //71
        public static Task<Packet> BanchoPrivileges(BanchoPermissions privileges) =>
            GetPacket(PacketType.BanchoPrivileges, (int) privileges);
        
        //72
        public static async Task<Packet> FriendList(List<int> friends)
        {
            await using var packet = PacketWriter.Create(PacketType.BanchoFriendList);
            packet.Write(friends);
            return packet.Dump();
        }
        
        //75
        public static Task<Packet> ProtocolVersion(int version = Misc.BanchoProtocolVersion) =>
            GetPacket(PacketType.BanchoProtocolVersion, version);
        
        //76 
        public static Task<Packet> MainMenuIcon(string icon, string websiteUrl) =>
            GetPacket(PacketType.BanchoMainMenuIcon, $"{icon}|{websiteUrl}");

        //81
        public static Task<Packet> MatchPlayerSkipped(int uid) => 
            GetPacket(PacketType.BanchoMatchPlayerSkipped, uid);

        //83
        public static Task<Packet> UserPresence(BanchoUserPresence presence) =>
            GetPacket(PacketType.BanchoUserPresence, presence);
        
        //83 overload
        public static async Task<Packet> UserPresence(Presence user) => 
            await UserPresence(await user.GetUserPresence());
        
        //83 bot
        public static Task<Packet> BotPresence()
        {
            var presence = new BanchoUserPresence
            {
                Id = Global.Bot.Id,
                Name = Global.Bot.Name,
                BanchoPermissions = BanchoPermissions.Bot,
                CountryCode = Global.BotCountry,
                Rank = 0,
                Timezone = 2,
                Longitude = 0.0f,
                Latitude = 0.0f
            };

            return UserPresence(presence);
        }

        //86
        public static Task<Packet> ServerRestart(int ms) => 
            GetPacket(PacketType.BanchoServerRestart, ms);

        //88
        public static Task<Packet> MatchInvite(Presence user, string to)
        {
            var message = new BanchoMessage
            {
                From = user.Name,
                To = to,
                Body = $"Hey! Let's play together!: {user.Match}",
                ClientId = user.Id
            };

            return GetPacket(PacketType.BanchoMatchInvite, message);
        }
        
        //89
        public static readonly Packet ChannelInfoEnd = new(PacketType.BanchoChannelInfoEnd);
        
        //91
        public static Task<Packet> MatchChangePassword(string newPassword) => 
            GetPacket(PacketType.BanchoMatchChangePassword, newPassword);

        //92
        public static Task<Packet> SilenceEnd(int time) => 
            GetPacket(PacketType.BanchoSilenceEnd, time);

        //94
        public static Task<Packet> UserSilenced(int uid) => 
            GetPacket(PacketType.BanchoUserSilenced, uid);

        //100
        public static Task<Packet> UserPmBlocked(string target)
        {
            var message = new BanchoMessage
            {
                From = "",
                To = target,
                Body = "",
                ClientId = 0
            };
            
            return GetPacket(PacketType.BanchoUserPmBlocked, message);
        }

        //101
        public static Task<Packet> TargetSilenced(string target)
        {
            var message = new BanchoMessage
            {
                From = "",
                To = target,
                Body = "",
                ClientId = 0
            };

            return GetPacket(PacketType.BanchoTargetIsSilenced, message);
        }

        //103
        public static Task<Packet> SwitchServer(int num) => GetPacket(PacketType.BanchoSwitchServer, num);

        //104 
        public static readonly Packet AccountRestricted = new(PacketType.BanchoAccountRestricted);

        //106
        public static readonly Packet MatchAbort = new(PacketType.BanchoMatchAbort);
        
        //107
        public static Task<Packet> SwitchTournamentServer(string ip) =>
            GetPacket(PacketType.BanchoSwitchTournamentServer, ip);

        private static async Task<Packet> GetPacket(PacketType type, int num)
        {
            await using var packet = PacketWriter.Create(type);
            packet.Write(num);
            return packet.Dump();
        }
        
        private static async Task<Packet> GetPacket(PacketType type, string str)
        {
            await using var packet = PacketWriter.Create(type);
            packet.Write(str);
            return packet.Dump();
        }
        
        private static async Task<Packet> GetPacket(PacketType type, ISerializable obj)
        {
            await using var packet = PacketWriter.Create(type);
            packet.WriteBanchoObject(obj);
            return packet.Dump();
        }
    }
}