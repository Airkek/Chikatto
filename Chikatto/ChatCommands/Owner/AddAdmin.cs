using System.Threading.Tasks;
using Chikatto.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.ChatCommands.Owner
{
    public class AddAdmin
    {
        [Command(new[] {"admin"}, "Make user admin", Privileges.Owner)]
        public static async Task<string> Handle(Presence user, string[] args)
        {
            if (args.Length != 1)
                return $"Usage: {Global.Config.CommandPrefix}admin <safe_name>";

            var username = Auth.GetSafeName(args[0]);

            var toAdmin = await Presence.FromDatabase(username);

            if (toAdmin is null)
                return $"User <{username}> not found";

            await toAdmin.AddPrivileges(Privileges.Admin);

            return $"{toAdmin} is now Admin";
        }
    }
}