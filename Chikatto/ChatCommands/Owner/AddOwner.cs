using System.Threading.Tasks;
using Chikatto.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.ChatCommands.Owner
{
    public class AddOwner
    {
        [Command(new[] {"owner"}, "Make user owner", Privileges.Owner)]
        public static async Task<string> Handle(Presence user, string[] args)
        {
            if (args.Length != 1)
                return $"Usage: {Global.Config.CommandPrefix}owner <safe_name>";

            var username = Auth.GetSafeName(args[0]);

            var toOwner = await Presence.FromDatabase(username);

            if (toOwner is null)
                return $"User <{username}> not found";

            await toOwner.AddPrivileges(Privileges.Owner);

            return $"{toOwner} is now Owner";
        }
    }
}