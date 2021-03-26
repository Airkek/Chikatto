using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chikatto.Database.Models;
using Chikatto.Objects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Chikatto.Utils
{
    public class Auth
    {
        public static async Task<User> Login(string name, string pwMd5)
        {
            var safe = GetSafeName(name);
            
            if(safe == Global.Bot.SafeName)
                return new User {Id = -1};
            
            int id;
            User user;
            if (!Global.IdCache.ContainsKey(safe))
            {
                var users = (await Global.Database.Users.AsNoTracking().ToListAsync()).Where(u => u.SafeName == safe).ToList();

                if (users.Count() != 1)
                    return new User {Id = -1};

                user = users.ElementAt(0);
                id = user.Id;

                Global.IdCache[user.SafeName] = id;
            }
            else
            {
                id = Global.IdCache[safe];

                if (Global.UserCache.ContainsKey(id))
                    user = Global.UserCache[id];
                else
                    user = await Global.Database.Users.FindAsync(id);
            }

            string bcrypt;

            if (Global.BCryptCache.ContainsKey(pwMd5))
                bcrypt = Global.BCryptCache[pwMd5];
            else
            {
                if (BCrypt.Net.BCrypt.Verify(pwMd5, user.Password))
                    bcrypt = user.Password;
                else
                    bcrypt = BCrypt.Net.BCrypt.HashPassword(pwMd5);
                
                Global.BCryptCache[pwMd5] = bcrypt;
            }

            if (bcrypt != user.Password)
                return new User {Id = -1};

            return user;
        }

        public static string GetSafeName(string name) => name.ToLower().Replace(" ", "_");
    }
}