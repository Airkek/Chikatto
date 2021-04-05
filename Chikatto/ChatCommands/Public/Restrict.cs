using System.Linq;
using System.Threading.Tasks;
using Chikatto.Constants;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.ChatCommands.Public
{
    public class Restrict
    {
        [Command(new[] {"restrict"}, "Restrict user", Privileges.Admin)]
        public static async Task<string> Handle(Presence user, string[] args)
        {
            if (args.Length < 2)
                return $"Usage: {Global.Config.CommandPrefix}restrict <safename> <reason>";

            var username = Auth.GetSafeName(args[0]);
            var reason = string.Join(" ", args.Skip(1));

            var toRestrict = await Presence.FromDatabase(username);

            if (toRestrict is null)
                return $"User <{username}> not found";
            
            if (toRestrict.Restricted)
                return $"{toRestrict} is already restricted";

            await toRestrict.Ban();

            return $"{toRestrict} has restricted for {reason}";
        }
    }
}