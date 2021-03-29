using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;
using Chikatto.Constants;
using Chikatto.Database;
using Chikatto.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Chikatto.Objects
{
    public class Presence
    {
        public User User;
        public Stats Stats;

        public int Id;
        public string Name;
        
        public string Token;
        
        public long LastPong = 0;
        public ConcurrentQueue<Packet> WaitingPackets = new ();

        public BanchoUserStatus Status = new ()
        {
            Action = BanchoAction.Idle, 
            Text = null, 
            MapMd5 = null, 
            Mods = Mods.NoMod,
            Mode = GameMode.Standard,
            MapId = 0
        };

        public BanchoUserStats GetStats()
        {
            return new()
            {
                Id = Id,
                Status = Status,
                RankedScore = Stats.rscore_vn_std,
                Accuracy = Stats.acc_vn_std,
                PlayCount = Stats.plays_vn_std,
                TotalScore = Stats.tscore_vn_std,
                Rank = 1,
                PP = (short) Stats.pp_vn_std
            };
        }

        public static async Task<Presence> FromDatabase(int id)
        {
            var user = await Global.Database.Users.FindAsync(id);
            
            if (user == null)
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
                Stats = await Global.Database.Stats.FindAsync(user.Id)
            };
        }
        
        public override string ToString() => $"<{Name} ({Id})>";
    }
}