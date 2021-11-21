using System.Threading.Tasks;
using Chikatto.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.ChatCommands.Owner
{
    public class AddMod
    {
        [Command(new[] {"mod"}, "Make user mod", Privileges.Owner)]
        public static async Task<string> Handle(Presence user, string[] args)
        {
            if (args.Length != 1)
                return $"Usage: {Global.Config.CommandPrefix}mod <safe_name>";

            var username = Auth.GetSafeName(args[0]);

            var toMod = await Presence.FromDatabase(username);

            if (toMod is null)
                return $"User <{username}> not found";

            await toMod.AddPrivileges(Privileges.Mod);

            return $"{toMod} is now Mod";
        }
    }
}