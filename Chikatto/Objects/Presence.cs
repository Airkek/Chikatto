﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Objects;
using Chikatto.Constants;
using Chikatto.Database;
using Chikatto.Database.Models;
using Chikatto.Extensions;
using Chikatto.Multiplayer;
using Chikatto.Utils;
using Chikatto.Enums;
using osu.Game.Beatmaps.Legacy;

namespace Chikatto.Objects
{
    public class Presence
    {
        public User User;
        public Stats Stats;

        public Score LastScore;
        public Beatmap LastNP;

        public int Id;
        public string Name;

        public byte CountryCode;

        public string Token;
        
        public long LastPong = 0;
        public readonly ConcurrentQueue<Packet> WaitingPackets = new();

        public bool InLobby = false;
        public bool Online = false;

        public ConcurrentDictionary<int, int> Friends;
        
        public readonly ConcurrentDictionary<string, Channel> JoinedChannels = new();
        
        public Match Match = null;
        
        public Presence Spectating;
        
        public readonly ConcurrentDictionary<int, Presence> Spectators = new();
        public Channel SpectateChannel;

        public bool Restricted;
        public long SilenceEnd;
        
        public BanchoUserStatus Status = new ()
        {
            Action = BanchoAction.Idle, 
            Text = null, 
            MapMd5 = null, 
            Mods = LegacyMods.None,
            Mode = GameMode.Standard,
            MapId = 0
        };
        
        public string ModeName;
        public string StatsTable;
        public int Rank;
        public long RankedScore;
        public long TotalScore;
        public int PlayCount;
        public short PP;
        public float Accuracy;

        public async Task CalculatePPFromBests(GameMode mode, bool isRelax)
        {
            var scores = 
                (await Db.FetchAll<Score>("SELECT pp, accuracy FROM scores JOIN beatmaps WHERE beatmaps.beatmap_md5 = scores.beatmap_md5 AND userid = @Id AND completed = 3 AND (ranked = 3 OR ranked = 2) AND mode = @mode AND is_relax = @isRelax AND disable_pp = false ORDER BY pp DESC LIMIT 100",
                    new
                    {
                        Id, 
                        isRelax,
                        mode
                    }))
                .ToArray();
            
            var pp = (ushort)scores.Select((t, i) => t.Performance * Math.Pow(0.95, i)).Sum();
            var acc = scores.Select(t => t.Accuracy).Average();

            var table = isRelax ? Misc.RelaxStatsColumn : Misc.NomodStatsColumn;

            var strmode = mode switch
            {
                GameMode.Standard => "std",
                GameMode.Taiko => "taiko",
                GameMode.Catch => "catch",
                GameMode.Mania => "mania",
                _ => "std"
            };

            await Db.Execute($"UPDATE {table} SET pp_{strmode} = @pp, avg_accuracy_{strmode} = @acc WHERE id = @Id", new
            {
                Id, acc, pp
            });
        }

        public async Task<int> GetRank()
        {
            return await Db.FetchOne<int>(
                $"SELECT COUNT({Misc.NomodStatsColumn}.id) FROM {Misc.NomodStatsColumn} JOIN users WHERE users.id = {Misc.NomodStatsColumn}.id AND pp_{ModeName.ToLower()} > @pp AND privileges & 1",
                new {pp = PP}) + 1;
        }

        public async Task SendMessage(string body, string sender, int senderId)
        {
            var message = new BanchoMessage
            {
                Body = body,
                From = sender,
                ClientId = senderId,
                To = Name
            };
            
            WaitingPackets.Enqueue(await FastPackets.SendMessage(message));
        }

        public Task SendMessage(string body, Presence user) => SendMessage(body, user.Name, user.Id);
        public Task SendMessage(string body, User user) => SendMessage(body, user.Name, user.Id);

        public async Task Notify(string message)
        {
            WaitingPackets.Enqueue(await FastPackets.Notification(message));
        }

        public async Task<BanchoUserStats> GetStats()
        {
            var stats = new BanchoUserStats
            {
                Id = Id,
                Status = Status,
                RankedScore = RankedScore,
                Accuracy = Accuracy,
                PlayCount = PlayCount,
                TotalScore = TotalScore,
                Rank = 0,
                PP = 0
            };
            
            if (!Restricted)
            {
                stats.Rank = Rank;
                stats.PP = PP;
            }
            
            return stats;
        }

        public async Task UpdateStats(GameMode gamemode, LegacyMods mods, bool force = false)
        {
            var mode = gamemode switch
            {
                GameMode.Standard => "STD",
                GameMode.Taiko => "Taiko",
                GameMode.Catch => "CTB",
                GameMode.Mania => "Mania",
                _ => "STD"
            };

            var table = !Global.Config.EnableRelax || (mods & LegacyMods.Relax) == 0 ? Misc.NomodStatsColumn : Misc.RelaxStatsColumn;

            if (mode != ModeName || table != StatsTable || force)
            {
                StatsTable = table;
                Stats = await Db.FetchOne<Stats>($"SELECT * FROM {StatsTable} WHERE id = @uid", new {uid = Id});
                
                ModeName = mode;

                // ReSharper disable once PossibleNullReferenceException
                RankedScore = (long) Global.GetPropertyFromStatsCache("RankedScore", mode).GetValue(Stats);
                
                // ReSharper disable once PossibleNullReferenceException
                TotalScore = (long) Global.GetPropertyFromStatsCache("TotalScore", mode).GetValue(Stats);
                
                // ReSharper disable once PossibleNullReferenceException
                Accuracy = (float) Global.GetPropertyFromStatsCache("Accuracy", mode).GetValue(Stats);
                
                // ReSharper disable once PossibleNullReferenceException
                PP = (short) Global.GetPropertyFromStatsCache("PP", mode).GetValue(Stats);
                
                // ReSharper disable once PossibleNullReferenceException
                PlayCount = (int) Global.GetPropertyFromStatsCache("Playcount", mode).GetValue(Stats);
                
                Rank = await GetRank();
            }
        }

        public async Task IncreasePlaycount(GameMode gamemode, LegacyMods mods)
        {
            var mode = gamemode switch
            {
                GameMode.Standard => "std",
                GameMode.Taiko => "taiko",
                GameMode.Catch => "ctb",
                GameMode.Mania => "mania",
                _ => "std"
            };
            
            var statsTable = !Global.Config.EnableRelax || (mods & LegacyMods.Relax) == 0 ? Misc.NomodStatsColumn : Misc.RelaxStatsColumn;
            
            await Db.Execute($"UPDATE {statsTable} SET playcount_{mode} = playcount_{mode} + 1 WHERE id = @Id", 
                new { Id });

            await UpdateStats(gamemode, mods, true);
        }

        public async Task UpdateStatus(BanchoUserStatus status)
        {
            Status = status;

            await UpdateStats(Status.Mode, Status.Mods);
        }

        public async Task<BanchoUserPresence> GetUserPresence()
        {
            var presence = new BanchoUserPresence
            {
                Id = Id,
                Name = Name,
                BanchoPermissions = await GetBanchoPermissions(),
                CountryCode = CountryCode,
                Rank = Rank,
                Timezone = 3,
                Longitude = 0.0f,
                Latitude = 0.0f
            };
            
            return presence;
        }

        public async Task<BanchoPermissions> GetBanchoPermissions()
        {
            return await GetBanchoPermissions(User);
        }

        public static async Task<BanchoPermissions> GetBanchoPermissions(User user)
        {
            var privs = BanchoPermissions.Normal;

            if ((user.Privileges & Privileges.Donor) != 0)
                privs |= BanchoPermissions.Supporter;

            if ((user.Privileges & Privileges.Nominator) != 0)
                privs |= BanchoPermissions.BAT;
            
            if ((user.Privileges & Privileges.Staff) != 0)
                privs |= BanchoPermissions.Moderator;

            if ((user.Privileges & Privileges.Owner) != 0)
                privs |= BanchoPermissions.Peppy;

            if ((user.Privileges & Privileges.TournamentStaff) != 0)
                privs |= BanchoPermissions.Tournament;

            return privs;
        }

        public static async Task<Presence> FromDatabase(int id)
        {
            var cached = Global.OnlineManager.GetById(id);
            if (cached is not null)
                return cached;
            
            var user = await Db.FetchOne<User>("SELECT * FROM users WHERE id = @uid", new { uid = id });
            
            if (user is null)
                return null;

            return await FromUser(user);
        }
        
        public static async Task<Presence> FromDatabase(string safename)
        {
            var cached = Global.OnlineManager.GetBySafeName(safename);
            if (cached is not null)
                return cached;
            
            var user = await Db.FetchOne<User>("SELECT * FROM users WHERE username_safe = @safe",new {safe = safename});
            if (user is null)
                return null;

            return await FromUser(user);
        }

        public static async Task<Presence> FromUser(User user)
        {
            var friendsDict = new ConcurrentDictionary<int, int>();
            var friendships = await Db.FetchAll<Friendships>("SELECT * FROM users_relationships WHERE user1 = @uid", new {uid = user.Id});
            friendships.Select(x => x.FriendId).ToList().ForEach(x => friendsDict[x] = x);

            friendsDict[Global.Bot.Id] = Global.Bot.Id;
            friendsDict[user.Id] = user.Id;

            var channel = new Channel($"#spectator_{user.Id}", "Spectator channel");
            Global.Channels[channel.TrueName] = channel;

            var stats = await Db.FetchOne<Stats>($"SELECT * FROM {Misc.NomodStatsColumn} WHERE id = @uid", new {uid = user.Id});

            if (stats is null)
            {
                XConsole.Log($"{user} - stats is null", back: ConsoleColor.Red);
                return null;
            }

            var presence = new Presence
            {
                Id = user.Id,
                Name = user.Name,
                User = user,
                CountryCode = Misc.CountryCodes.ContainsKey(stats.Country.ToUpper()) ? Misc.CountryCodes[stats.Country.ToUpper()] : (byte) 0,
                Stats = stats,
                StatsTable = "users_stats",
                Friends = friendsDict,
                SpectateChannel = channel,
                Restricted = (user.Privileges & (Privileges.Public | Privileges.PendingVerification)) == 0,
                SilenceEnd = user.SilenceEnd
            };

            await presence.UpdateStatus(presence.Status); // init rank, pp, score etc
            return presence;
        }

        public async Task AddFriend(int id)
        {
            if(id == Global.Bot.Id || Friends.ContainsKey(id))
                return;

            Friends[id] = id;
            await Db.Execute("INSERT INTO users_relationships (user1, user2) VALUES (@uid, @fid)", new { uid = Id, fid = id });
        }
        
        public async Task RemoveFriend(int id)
        {
            if(id == Global.Bot.Id || !Friends.ContainsKey(id))
                return;

            Friends.Remove(id);
            await Db.Execute("DELETE FROM users_relationships WHERE user1 = @uid AND user2 = @fid", 
                new { uid = Id, fid = id });
        }

        public int SilenceEndRelative => (int)(SilenceEnd - DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        public bool Silenced => SilenceEndRelative > 0;

        public async Task Mute(int seconds, string reason)
        {
            SilenceEnd = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + seconds;
            
            await Db.Execute("UPDATE users SET silence_reason = @reason, silence_end = @seconds WHERE id = @uid",
                new {uid = Id, seconds = (int)SilenceEnd, reason});

            if (Online)
            {
                if (Match is not null) await Match.Leave(this);
                
                InLobby = false;
                await Global.Channels["#lobby"].RemoveUser(this);
                
                await Notify("You has been silenced");
                WaitingPackets.Enqueue(await FastPackets.SilenceEnd(seconds));

                await Global.OnlineManager.AddPacketToAllUsers(await FastPackets.UserSilenced(Id));
            }

            XConsole.Log($"{ToString()} has silenced ({seconds} seconds)", back: ConsoleColor.Magenta);
        }

        public async Task Ban(bool restrict = true)
        {
            var newPriv = User.Privileges & ~Privileges.Public;
            
            if (!restrict)
                newPriv &= ~Privileges.Normal;

            await UpdatePrivileges(newPriv);

            Restricted = true;
            
            if (Online)
            {
                if (restrict)
                {
                    await Notify("Your account is currently in restricted mode");
                    WaitingPackets.Enqueue(FastPackets.AccountRestricted);
                }
                else
                {
                    await Notify("Goodbye...");
                    WaitingPackets.Enqueue(await FastPackets.UserId(-3));
                }
                
                await Global.OnlineManager.AddPacketToAllUsers(await FastPackets.Logout(Id));
            }

            var logStr = restrict ? "restricted" : "banned";
            XConsole.Log($"{ToString()} has {logStr}", back: ConsoleColor.Magenta);
        }

        public async Task Unban()
        {
            await AddPrivileges(Privileges.Normal | Privileges.Public);

            Restricted = false;

            if (Online)
            {
                await Notify("You just been unrestricted!");
                WaitingPackets.Enqueue(await FastPackets.ServerRestart(0));
            }

            XConsole.Log($"{ToString()} has unbanned", back: ConsoleColor.Magenta);
        }
        
        public Task AddPrivileges(Privileges priv)
        {
            var newPriv = User.Privileges | priv;
            return UpdatePrivileges(newPriv);
        }

        public Task RemovePrivileges(Privileges priv)
        {
            var newPriv = User.Privileges & ~priv;
            return UpdatePrivileges(newPriv);
        }

        public async Task UpdatePrivileges(Privileges newPriv)
        {
            User.Privileges = newPriv;
            await Db.Execute("UPDATE users SET privileges = @newPriv where id = @id", new { newPriv, Id });
        }
        
        public async Task AddSpectator(Presence user)
        {
            if(Spectators.ContainsKey(user.Id))
                return;

            if (Spectators.IsEmpty)
                await SpectateChannel.JoinUser(this);
            else
                await AddPacketToSpectators(await FastPackets.FellowSpectatorJoined(user.Id));

            Spectators[user.Id] = user;
            await SpectateChannel.JoinUser(user);
            
            WaitingPackets.Enqueue(await FastPackets.SpectatorJoined(user.Id));
        }
        
        public async Task RemoveSpectator(Presence user)
        {
            if(!Spectators.ContainsKey(user.Id))
                return;

            Spectators.Remove(user.Id);
            await SpectateChannel.RemoveUser(user);

            if (Spectators.IsEmpty)
                await SpectateChannel.RemoveUser(this);
            else
                await AddPacketToSpectators(await FastPackets.FellowSpectatorLeft(user.Id));

            WaitingPackets.Enqueue(await FastPackets.SpectatorLeft(user.Id));
        }

        public async Task AddPacketToSpectators(Packet packet)
        {
            foreach (var (_, spectator) in Spectators)
                spectator.WaitingPackets.Enqueue(packet);
        }

        public async Task DropSpectators()
        {
            foreach (var (_, spectator) in Spectators)
                await RemoveSpectator(spectator); 
        }

        public override string ToString() => $"<{Name} ({Id})>";
    }
}