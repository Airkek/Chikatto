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

            return new User
            {
                Id = Global.Test[safe],
                Name = name, //TODO: dbName
                SafeName = safe,
                BanchoToken = RandomFabric.GenerateBanchoToken(),
                LastPong = DateTime.Now.Second
            };
        }

        public static string GetSafeName(string name) => name.ToLower().Replace(" ", "_");
    }
}