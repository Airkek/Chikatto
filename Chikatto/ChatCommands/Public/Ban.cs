using System.Linq;
using System.Threading.Tasks;
using Chikatto.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.ChatCommands.Public
{
    public class Ban
    {
        [Command(new[] {"ban"}, "Ban user", Privileges.Admin)]
        public static async Task<string> Handle(Presence user, string[] args)
        {
            if (args.Length < 2)
                return $"Usage: {Global.Config.CommandPrefix}ban <safename> <reason>";

            var username = Auth.GetSafeName(args[0]);
            var reason = string.Join(" ", args.Skip(1));

            var toBan = await Presence.FromDatabase(username);

            if (toBan is null)
                return $"User <{username}> not found";

            if (toBan.Restricted && (toBan.User.Privileges & Privileges.Restricted) == 0)
                return $"{toBan} is already banned";

            await toBan.Ban(false);

            return $"{toBan} has banned for {reason}";
        }
    }
}