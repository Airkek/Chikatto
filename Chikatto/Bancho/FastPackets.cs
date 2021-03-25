using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;
using Chikatto.Constants;
using Chikatto.Objects;

namespace Chikatto.Utils
{
    public static class FastPackets
    {
        //5
        public static Packet UserId(int id) => new (PacketType.BanchoUserId, BitConverter.GetBytes(id));
        
        //7
        public static Packet SendMessage(BanchoMessage message)
        {
            using var packet = new WriteablePacket(PacketType.BanchoSendMessage);
            packet.Writer.WriteBanchoObject(message);
            return packet.Dump();
        }
        
        //8
        public static Packet Ping() => new Packet(PacketType.BanchoPing);
        
        //9
        public static Packet ChangeUsername(string oldName, string newName)
        {
            using var packet = new WriteablePacket(PacketType.BanchoIrcChangeUsername);
            packet.Writer.Write($"{oldName}>>>>{newName}");
            return packet.Dump();
        }
        
        //11
        public static Packet UserStats(BanchoUserStats stats)
        {
            using var packet = new WriteablePacket(PacketType.BanchoUserStats);
            packet.Writer.WriteBanchoObject(stats);
            return packet.Dump();
        }
        //11 overload
        public static Packet UserStats(User user)
        {
            //TODO: UserStats from user
            var stats = new BanchoUserStats
            {
                Id = user.Id,
                Accuracy = 13.37f,
                Action = 3,
                Text = "Chikatto's source code",
                Rank = 1337,
                MapId = 0,
                MapMd5 = "",
                PlayCount = 1337,
                PP = 1337,
                RankedScore = 1337,
                TotalScore = 1337,
                Mode = GameMode.Standard,
                Mods = 0
            };
            return UserStats(stats);
        }
        public static Packet BotStats()
        {
            var stats = new BanchoUserStats
            {
                Id = 1,
                Accuracy = 13.37f,
                Action = 3,
                Text = "Chikatto's source code",
                Rank = 1337,
                MapId = 0,
                MapMd5 = "",
                PlayCount = 1337,
                PP = 1337,
                RankedScore = 1337,
                TotalScore = 1337,
                Mode = GameMode.Standard,
                Mods = 0
            };
            return UserStats(stats);
        }
        
        //12
        public static Packet Logout(int uid) 
        {
            using var packet = new WriteablePacket(PacketType.BanchoUserLogout);
            packet.Writer.Write(uid);
            packet.Writer.Write((byte) 0);
            return packet.Dump();
        }
        
        //14
        public static Packet SpectatorJoined(int id) =>
            new Packet(PacketType.BanchoSpectatorJoined, BitConverter.GetBytes(id));
        
        //14
        public static Packet SpectatorLeft(int id) =>
            new Packet(PacketType.BanchoSpectatorLeft, BitConverter.GetBytes(id));
        
        //15
        public static Packet SpectateFrames(byte[] frames) => new Packet(PacketType.BanchoSpectateFrames, frames);
        
        //19
        public static Packet VersionUpdate() => new Packet(PacketType.BanchoVersionUpdate); 
        
        //22
        public static Packet CantSpectate(int id) => 
            new Packet(PacketType.BanchoCantSpectate, BitConverter.GetBytes(id));
        
        //23
        public static Packet GetAttention() => new Packet(PacketType.BanchoGetAttention);

        //24
        public static Packet Notification(string text)
        {
            using var packet = new WriteablePacket(PacketType.BanchoNotification);
            packet.Writer.Write(text);
            return packet.Dump();
        }
        
        //24
        public static Packet NewMatch(BanchoMatch banchoMatch)
        {
            using var packet = new WriteablePacket(PacketType.BanchoNewMatch);
            packet.Writer.WriteBanchoObject(banchoMatch);
            return packet.Dump();
        }
        
        //26
        public static Packet UpdateMatch(BanchoMatch banchoMatch, bool sendPassword = true)
        {
            using var packet = new WriteablePacket(PacketType.BanchoUpdateMatch);
            packet.Writer.WriteBanchoObject(banchoMatch);
            return packet.Dump();
        }
        
        //28
        public static Packet DisposeMatch(int id) =>
            new Packet(PacketType.BanchoDisposeMatch, BitConverter.GetBytes(id));
        
        //34
        public static Packet ToggleBlockNonFriendPm() => new Packet(PacketType.BanchoToggleBlockNonFriendPm);
        
        //36
        public static Packet MatchJoinSuccess(BanchoMatch banchoMatch)
        {
            using var packet = new WriteablePacket(PacketType.BanchoMatchJoinSuccess);
            packet.Writer.WriteBanchoObject(banchoMatch);
            return packet.Dump();
        }
        
        //37
        public static Packet MatchJoinFail() => new Packet(PacketType.BanchoMatchJoinFail);
        
        //42
        public static Packet FellowSpectatorJoined(int id) =>
            new Packet(PacketType.BanchoFellowSpectatorJoined, BitConverter.GetBytes(id));
        
        //43
        public static Packet FellowSpectatorLeft(int id) =>
            new Packet(PacketType.BanchoFellowSpectatorLeft, BitConverter.GetBytes(id));
        
        //45: unused
        
        //46
        public static Packet MatchStart(BanchoMatch banchoMatch)
        {
            using var packet = new WriteablePacket(PacketType.BanchoMatchStart);
            packet.Writer.WriteBanchoObject(banchoMatch);
            return packet.Dump();
        }
        
        //48
        public static Packet MatchScoreUpdate( /*ScoreFrame frame*/)
        {
            throw new NotImplementedException(); //TODO: MatchScoreUpdate
        }
        
        //50
        public static Packet MatchTransferHost() => new Packet(PacketType.BanchoMatchTransferHost);
        
        //53
        public static Packet MatchAllPlayersLoaded() => new Packet(PacketType.BanchoMatchAllPlayersLoaded);
        
        //57
        public static Packet MatchPlayerFailed(int slotId) =>
            new Packet(PacketType.BanchoMatchPlayerFailed, BitConverter.GetBytes(slotId));
        
        //58
        public static Packet MatchComplete() => new Packet(PacketType.BanchoMatchComplete);
        
        //61
        public static Packet MatchSkip() => new Packet(PacketType.BanchoMatchSkip);
        
        //62: unused
        
        //64
        public static Packet ChannelJoinSuccess(string channelName)
        {
            using var packet = new WriteablePacket(PacketType.BanchoChannelJoinSuccess);
            packet.Writer.Write(channelName);
            return packet.Dump();
        }

        //65
        public static Packet ChannelInfo(string channelName, string topic, int playerCount)
        {
            using var packet = new WriteablePacket(PacketType.BanchoChannelInfo);
            packet.Writer.Write(channelName);
            packet.Writer.Write(topic);
            packet.Writer.Write(playerCount);
            return packet.Dump();
        }
        
        //66
        public static Packet ChannelKick(string channelName)
        {
            using var packet = new WriteablePacket(PacketType.BanchoChannelKick);
            packet.Writer.Write(channelName);
            return packet.Dump();
        }
        
        //67
        public static Packet ChannelAutoJoin(string channelName, string topic, int playerCount)
        {
            using var packet = new WriteablePacket(PacketType.BanchoChannelAutoJoin);
            packet.Writer.Write(channelName);
            packet.Writer.Write(topic);
            packet.Writer.Write(playerCount);
            return packet.Dump();
        }
        
        //69
        public static Packet BeatmapInfoReply( /*List<Map> maps*/)
        {
            throw new NotImplementedException(); // TODO: BeatmapInfoReply
        }

        //71
        public static Packet BanchoPrivileges(int privileges) =>
            new Packet(PacketType.BanchoPrivileges, BitConverter.GetBytes(privileges));
        
        //72
        public static Packet FriendList(List<int> friends)
        {
            using var packet = new WriteablePacket(PacketType.BanchoFriendList);
            packet.Writer.Write(friends);
            return packet.Dump();
        }
        
        //75
        public static Packet ProtocolVersion(int version) =>
            new Packet(PacketType.BanchoProtocolVersion, BitConverter.GetBytes(version));
        
        //76 TODO: overload and MainMenuIcon class (for config)
        public static Packet MainMenuIcon(string icon) //should be $"{iconUrl}|{websiteUrl}"; 
        {
            using var packet = new WriteablePacket(PacketType.BanchoMainMenuIcon);
            packet.Writer.Write(icon);
            return packet.Dump();
        }
        
        //80: unused
        
        //81
        public static Packet MatchPlayerSkipped(int uid) =>
            new Packet(PacketType.BanchoMatchPlayerSkipped, BitConverter.GetBytes(uid));

        //83
        public static Packet UserPresence(BanchoUserPresence presence)
        {
            using var packet = new WriteablePacket(PacketType.BanchoUserPresence);
            packet.Writer.WriteBanchoObject(presence);
            return packet.Dump();
        }
        //83 overload
        public static Packet UserPresence(User user)
        {
            throw new NotImplementedException();
        }
        //83 bot //TODO bot config
        public static Packet BotPresence()
        {
            var presence = new BanchoUserPresence
            {
                Id = 1,
                Name = "DenBai",
                BanchoPrivileges = 19,
                CountryCode = 222,
                Rank = 0,
                Timezone = 26,
                X = 0.0,
                Y = 0.0
            };
            return UserPresence(presence);
        }

        //86
        public static Packet ServerRestart(int ms) => new(PacketType.BanchoServerRestart, BitConverter.GetBytes(ms));
        
        //88
        public static Packet MatchInvite(User user, string to)
        {
            var message = new BanchoMessage
            {
                From = user.Name,
                To = to,
                Body = $"Hey! Let's play together!: ", //TODO: match embed
                ClientId = user.Id
            };
            using var packet = new WriteablePacket(PacketType.BanchoMatchInvite);
            packet.Writer.WriteBanchoObject(message);
            return packet.Dump();
        }
        
        //89
        public static Packet ChannelInfoEnd() => new Packet(PacketType.BanchoChannelInfoEnd);
        
        //91
        public static Packet MatchChangePassword(string newPassword)
        {
            using var packet = new WriteablePacket(PacketType.BanchoMatchChangePassword);
            packet.Writer.Write(newPassword);
            return packet.Dump();
        }

        //92
        public static Packet SilenceEnd(int time) =>
            new Packet(PacketType.BanchoSilenceEnd, BitConverter.GetBytes(time));
        
        //94
        public static Packet UserSilenced(int uid) => 
            new Packet(PacketType.BanchoUserSilenced, BitConverter.GetBytes(uid));
        
        //95
        public static Packet UserPresenceSingle(int uid) => 
            new Packet(PacketType.BanchoUserPresenceSingle, BitConverter.GetBytes(uid));

        //96
        public static Packet UserPresenceBundle(List<int> uids) 
        {
            using var packet = new WriteablePacket(PacketType.BanchoUserPresenceBundle);
            packet.Writer.Write(uids);
            return packet.Dump();
        }
        
        //100
        public static Packet UserPmBlocked(string target)
        {
            var message = new BanchoMessage
            {
                From = "",
                To = target,
                Body = "",
                ClientId = 0
            };
            using var packet = new WriteablePacket(PacketType.BanchoUserPmBlocked);
            packet.Writer.WriteBanchoObject(message);
            return packet.Dump();
        }

        //101
        public static Packet TargetSilenced(string target)
        {
            var message = new BanchoMessage
            {
                From = "",
                To = target,
                Body = "",
                ClientId = 0
            };
            using var packet = new WriteablePacket(PacketType.BanchoTargetIsSilenced);
            packet.Writer.WriteBanchoObject(message);
            return packet.Dump();
        }

        //103
        public static Packet SwitchServer(int num) =>
            new Packet(PacketType.BanchoSwitchServer, BitConverter.GetBytes(num));
        
        //104 
        public static Packet AccountRestricted() => new Packet(PacketType.BanchoAccountRestricted);
        
        //105 unused
        
        //106
        public static Packet MatchAbort() => new Packet(PacketType.BanchoMatchAbort);
        
        //107
        public static Packet SwitchTournamentServer(string ip)
        {
            using var packet = new WriteablePacket(PacketType.BanchoSwitchTournamentServer);
            packet.Writer.Write(ip);
            return packet.Dump();
        }
    }
}