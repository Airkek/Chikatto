using System.Linq;
using System.Threading.Tasks;
using Chikatto.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.ChatCommands.Mod
{
    public class Silence
    {
        private static readonly string Usage = $"Usage: {Global.Config.CommandPrefix}silence <safe_name> <1s/1min/1h/1d/1m/1y> <reason>";
        
        [Command(new[] {"silence", "mute"}, "Mute user", Privileges.Mod)]
        public static async Task<string> Handle(Presence user, string[] args)
        {
            if (args.Length < 3)
                return Usage;

            var username = Auth.GetSafeName(args[0]);
            var timeStr = args[1].ToLower();
            var reason = string.Join(" ", args.Skip(2));

            var timeTotalSeconds = TimeHelper.ParseTime(timeStr);
            
            if (timeTotalSeconds <= 0)
                return $"Error: time must be bigger than 0\n{Usage}";
            
            var toMute = await Presence.FromDatabase(username);

            if (toMute is null)
                return $"User <{username}> not found";
            
            if (toMute.Silenced)
                return $"{toMute} is already silenced";

            await toMute.Mute(timeTotalSeconds, reason);

            return $"{toMute} has silenced for {reason}";
        }
    }
}