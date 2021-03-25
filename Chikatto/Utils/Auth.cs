using System;
using Chikatto.Objects;

namespace Chikatto.Utils
{
    public class Auth
    {
        public static User Login(string name, string psMd5)
        {
            var safe = GetSafeName(name);
            if (!Global.Test.ContainsKey(safe)) // :D
                return new User { Id = -1 };

            var id = Global.Test[safe];

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
                    LastPong = 0,
                    BanchoToken = RandomFabric.GenerateBanchoToken()
                };
            }

            return user;
        }

        public static string GetSafeName(string name) => name.ToLower().Replace(" ", "_");
    }
}