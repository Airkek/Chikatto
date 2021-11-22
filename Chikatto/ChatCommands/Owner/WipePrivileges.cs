using System.Threading.Tasks;
using Chikatto.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.ChatCommands.Owner
{
    public class WipePrivileges
    {
        [Command(new[] {"wipeprivs"}, "Wipe user privileges", Privileges.Owner)]
        public static async Task<string> Handle(Presence user, string[] args)
        {
            if (args.Length != 1)
                return $"Usage: {Global.Config.CommandPrefix}wipeprivs <safe_name>";

            var username = Auth.GetSafeName(args[0]);

            var toWipe = await Presence.FromDatabase(username);

            if (toWipe is null)
                return $"User <{username}> not found";

            var newPriv = Privileges.Normal | Privileges.Public;
            
            if ((toWipe.User.Privileges & Privileges.Donor) != 0)
                newPriv |= Privileges.Donor;
            
            await toWipe.UpdatePrivileges(newPriv);

            return $"Wiped {toWipe} privileges";
        }
    }
}