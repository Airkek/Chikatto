using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
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

        public static async Task<Presence> FromDatabase(int id)
        {
            var user = await Global.Database.Users.FindAsync(id);
            
            if (user == null)
                return null;

            return new Presence 
            {
                Id = user.Id,
                Name = user.Name,
                User = user, 
                Stats = await Global.Database.Stats.FindAsync(id)
            };
        }

        public static async Task<Presence> FromDatabase(string safename)
        {
            if (!await Global.Database.Users.AnyAsync(x => x.SafeName == safename))
                return null;
            
            var user = await Global.Database.Users.FirstAsync(x => x.SafeName == safename);

            return new Presence 
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