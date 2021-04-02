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

namespace Chikatto.Objects
{
    public class Presence
    {
        public User User;
        public Stats Stats;

        public int Id;
        public string Name;

        public byte CountryCode;

        public string Token;
        
        public long LastPong = 0;
        public readonly ConcurrentQueue<Packet> WaitingPackets = new();

        public bool InLobby = false;

        public ConcurrentDictionary<int, int> Friends;
        
        public readonly ConcurrentDictionary<string, Channel> JoinedChannels = new();
        public Match Match = null;

        public BanchoUserStatus Status = new ()
        {
            Action = BanchoAction.Idle, 
            Text = null, 
            MapMd5 = null, 
            Mods = Mods.NoMod,
            Mode = GameMode.Standard,
            MapId = 0
        };

        public async Task<int> GetRank()
        {
            return (await Db.FetchAll<Stats>("SELECT * FROM stats")).Count(x => x.pp_vn_std > Stats.pp_vn_std) + 1;
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
            return new()
            {
                Id = Id,
                Status = Status,
                RankedScore = Stats.rscore_vn_std,
                Accuracy = Stats.acc_vn_std,
                PlayCount = Stats.plays_vn_std,
                TotalScore = Stats.tscore_vn_std,
                Rank = await GetRank(),
                PP = (short) Stats.pp_vn_std
            };
        }

        public async Task<BanchoUserPresence> GetUserPresence()
        {
            var presence = new BanchoUserPresence
            {
                Id = Id,
                Name = Name,
                BanchoPermissions = await GetBanchoPermissions(),
                CountryCode = CountryCode,
                Rank = await GetRank(),
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
            var privs = BanchoPermissions.User;

            if ((user.Privileges & Privileges.Nominator) != 0)
                privs |= BanchoPermissions.BAT;
            
            if ((user.Privileges & Privileges.Staff) != 0)
                privs |= BanchoPermissions.Moderator;

            if ((user.Privileges & Privileges.Dangerous) != 0)
                privs |= BanchoPermissions.Peppy;

            if ((user.Privileges & Privileges.Tournament) != 0)
                privs |= BanchoPermissions.Tournament;

            return privs;
        }

        public static async Task<Presence> FromDatabase(int id)
        {
            var user = await Db.FetchOne<User>("SELECT * FROM users WHERE id = @uid", new { uid = id });
            
            if (user is null)
                return null;

            return await FromUser(user);
        }

        public static async Task<Presence> FromDatabase(string safename)
        {
            var user = await Db.FetchOne<User>("SELECT * FROM users WHERE safe_name = @safe",new {safe = safename});
            if (user is null)
                return null;

            return await FromUser(user);
        }

        public static async Task<Presence> FromUser(User user)
        {
            
            var friendsDict = new ConcurrentDictionary<int, int>();
            var x = await Db.FetchAll<Friendships>("SELECT * FROM friendships WHERE user1 = @uid", new {uid = user.Id});
            x
                .Select(x => x.FriendId).ToList().ForEach(x => friendsDict[x] = x);

            friendsDict[Global.Bot.Id] = Global.Bot.Id;

            return new()
            {
                Id = user.Id,
                Name = user.Name,
                User = user,
                CountryCode = Misc.CountryCodes.ContainsKey(user.Country.ToUpper()) ? Misc.CountryCodes[user.Country.ToUpper()] : (byte) 0,
                Stats = await Db.FetchOne<Stats>("SELECT * FROM stats WHERE id = @uid", new { uid = user.Id }),
                Friends = friendsDict
            };
        }

        public async Task AddFriend(int id)
        {
            if(id == Global.Bot.Id || Friends.ContainsKey(id))
                return;

            Friends[id] = id;
            await Db.Execute("INSERT INTO friendships VALUE (@uid, @fid)", new { uid = Id, fid = id });
        }
        
        public async Task RemoveFriend(int id)
        {
            if(id == Global.Bot.Id || !Friends.ContainsKey(id))
                return;

            Friends.Remove(id);
            await Db.Execute("DELETE FROM friendships WHERE user1 = @uid AND user2 = @fid", new { uid = Id, fid = id });
        }

        public override string ToString() => $"<{Name} ({Id})>";
    }
}