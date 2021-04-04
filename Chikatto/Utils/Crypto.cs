using System.Text;
using Chikatto.Objects;

namespace Chikatto.Utils
{
    public static class Crypto
    {
        public static string Bcrypt(string plain, string verify = null)
        {
            string bcrypt;

            if (Global.BCryptCache.ContainsKey(plain))
                bcrypt = Global.BCryptCache[plain];
            else
            {
                if (verify is not null && BCrypt.Net.BCrypt.Verify(plain, verify))
                    bcrypt = verify;
                else
                    bcrypt = BCrypt.Net.BCrypt.HashPassword(plain);
                
                Global.BCryptCache[plain] = bcrypt;
            }

            return bcrypt;
        }
    }
}