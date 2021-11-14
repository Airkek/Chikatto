using System;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Multiplayer;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events.Types
{
    public class MatchInvite
    {
        [Event(PacketType.OsuMatchInvite, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            if (user.Match is null)
                return;

            var id = reader.ReadInt32();

            var target = Global.OnlineManager.GetById(id);

            if (target is null)
            {
                await user.Notify("User is offline");
                return;
            }
            
            if (target.Silenced)
            {
                await user.Notify("User silenced");
                return;
            }
            
            target.WaitingPackets.Enqueue(await FastPackets.MatchInvite(user, target.Name));

            XConsole.Log($"{user} invited {target} to {user.Match}", ConsoleColor.Green);
        }
    }
}