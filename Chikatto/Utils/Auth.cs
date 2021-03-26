using System;
using Chikatto.Database.Models;
using Chikatto.Objects;

namespace Chikatto.Utils
{
    public class Auth
    {
        public static User Login(string name, string psMd5)
        {
            var safe = GetSafeName(name);
            if (!Global.IdCache.ContainsKey(safe)) // :D
                return new User { Id = -1 };

            var id = Global.IdCache[safe];

            User user;
            if (Global.UserCache.ContainsKey(id))
                user = Global.UserCache[id];
            else 
            {
                user = new User
                {
                    Id = id,
                    Name = name, //TODO: dbName
                    SafeName = safe,
                    LastPong = 0
                };
            }

            return user;
        }

        public static string GetSafeName(string name) => name.ToLower().Replace(" ", "_");
    }
}