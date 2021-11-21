using System.Threading.Tasks;
using Chikatto.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.ChatCommands.Owner
{
    public class AddBN
    {
        [Command(new[] {"bn", "bat", "nominator"}, "Make user Beatmap Nominator", Privileges.Owner)]
        public static async Task<string> Handle(Presence user, string[] args)
        {
            if (args.Length != 1)
                return $"Usage: {Global.Config.CommandPrefix}bn <safe_name>";

            var username = Auth.GetSafeName(args[0]);

            var toBn = await Presence.FromDatabase(username);

            if (toBn is null)
                return $"User <{username}> not found";

            await toBn.AddPrivileges(Privileges.Nominator);

            return $"{toBn} is now BN";
        }
    }
}