using System.Linq;
using System.Threading.Tasks;
using Chikatto.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.ChatCommands.Mod
{
    public class Unsilence
    {
        [Command(new[] {"unsilence", "unmute"}, "Unmute user", Privileges.Mod)]
        public static async Task<string> Handle(Presence user, string[] args)
        {
            if (args.Length != 1)
                return $"Usage: {Global.Config.CommandPrefix}unsilence <safename>";

            var username = Auth.GetSafeName(args[0]);

            var toUnmute = await Presence.FromDatabase(username);

            if (toUnmute is null)
                return $"User <{username}> not found";
            
            if (!toUnmute.Silenced)
                return $"{toUnmute} is not silenced";

            await toUnmute.Mute(-toUnmute.SilenceEndRelative, ""); // :D

            return $"{toUnmute} has unsilenced";
        }
    }
}