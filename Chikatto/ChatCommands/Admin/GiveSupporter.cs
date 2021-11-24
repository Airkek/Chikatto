using System;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Database;
using Chikatto.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.ChatCommands.Admin
{
    public class GiveSupporter
    {
        private static readonly string Usage = $"Usage: {Global.Config.CommandPrefix}supporter <safe_name> <1s/1min/1h/1d/1m/1y>";
        
        [Command(new[] {"supporter"}, "Give supporter to user", Privileges.Admin)]
        public static async Task<string> Handle(Presence user, string[] args)
        {
            if (args.Length != 2)
                return Usage;

            var username = Auth.GetSafeName(args[0]);
            var timeStr = args[1].ToLower();

            var timeTotalSeconds = TimeHelper.ParseTime(timeStr);
            
            if (timeTotalSeconds <= 0)
                return $"Error: time must be bigger than 0\n{Usage}";
            
            var toSupport = await Presence.FromDatabase(username);

            if (toSupport is null)
                return $"User <{username}> not found";

            await toSupport.AddPrivileges(Privileges.Donor);
            
            await Db.Execute("UPDATE users SET donor_expire = @exp WHERE id = @uid", 
                new{uid = toSupport.Id, exp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + timeTotalSeconds});

            return $"Given supporter to {toSupport} for {timeStr}";
        }
    }
}