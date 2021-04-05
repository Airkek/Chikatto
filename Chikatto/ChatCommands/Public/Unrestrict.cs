using System.Linq;
using System.Threading.Tasks;
using Chikatto.Constants;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.ChatCommands.Public
{
    public class Unrestrict
    {
        [Command(new[] {"unrestrict", "unban"}, "Unrestrict/unban user", Privileges.Admin)]
        public static async Task<string> Handle(Presence user, string[] args)
        {
            if (args.Length != 1)
                return $"Usage: {Global.Config.CommandPrefix}unrestrict <safename>";

            var username = Auth.GetSafeName(args[0]);
            var reason = string.Join(" ", args.Skip(1));

            var toUnrestrict = await Presence.FromDatabase(username);

            if (toUnrestrict is null)
                return $"User <{username}> not found";

            if (!toUnrestrict.Restricted)
                return $"{toUnrestrict} is not restricted";
            
            await toUnrestrict.Unban();

            return $"{toUnrestrict} has unrestricted";
        }
    }
}