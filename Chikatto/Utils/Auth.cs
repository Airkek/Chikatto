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
        public static async Task<Presence> Login(string name, string pwMd5)
        {
            var safe = GetSafeName(name);
            
            if(safe == Global.Bot.SafeName)
                return null;
            
            var user = await Presence.FromDatabase(GetSafeName(name));

            if (user == null)
                return null;

            string bcrypt;

            if (Global.BCryptCache.ContainsKey(pwMd5))
                bcrypt = Global.BCryptCache[pwMd5];
            else
            {
                if (BCrypt.Net.BCrypt.Verify(pwMd5, user.User.Password))
                    bcrypt = user.User.Password;
                else
                    bcrypt = BCrypt.Net.BCrypt.HashPassword(pwMd5);
                
                Global.BCryptCache[pwMd5] = bcrypt;
            }

            return bcrypt == user.User.Password ? user : null;
        }

        public static string GetSafeName(string name) => name.ToLower().Replace(" ", "_");
    }
}