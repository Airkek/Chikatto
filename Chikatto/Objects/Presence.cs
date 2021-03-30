using System.Collections.Concurrent;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Objects;
using Chikatto.Constants;
using Chikatto.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Chikatto.Objects
{
    public class Presence
    {
        public User User;
        public Stats Stats;

        public int Id;
        public string Name;

        public byte CountryCode;

        public BanchoPermissions Permissions;
        
        public string Token;
        
        public long LastPong = 0;
        public ConcurrentQueue<Packet> WaitingPackets = new();
        
        public ConcurrentDictionary<string, Channel> JoinedChannels = new();

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
            return await Global.Database.Stats.CountAsync(x => x.pp_vn_std > Stats.pp_vn_std) + 1;
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
            var user = await Global.Database.Users.FindAsync(id);
            
            if (user is null)
                return null;

            return await FromUser(user);
        }

        public static async Task<Presence> FromDatabase(string safename)
        {
            if (!await Global.Database.Users.AnyAsync(x => x.SafeName == safename))
                return null;
            
            var user = await Global.Database.Users.FirstAsync(x => x.SafeName == safename);

            return await FromUser(user);
        }

        public static async Task<Presence> FromUser(User user)
        {
            return new()
            {
                Id = user.Id,
                Name = user.Name,
                User = user,
                Permissions = await GetBanchoPermissions(user),
                CountryCode = Misc.CountryCodes.ContainsKey(user.Country.ToUpper()) ? Misc.CountryCodes[user.Country.ToUpper()] : (byte) 0,
                Stats = await Global.Database.Stats.FindAsync(user.Id)
            };
        }

        public override string ToString() => $"<{Name} ({Id})>";
    }
}