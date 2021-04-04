using System;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events.Types
{
    public class SpectatorLeft
    {
        [Event(PacketType.OsuStopSpectating, false)]
        public static async Task Handle(PacketReader _, Presence user)
        {
            var toSpec = user.Spectating;

            if (toSpec is null)
            {
                XConsole.Log($"{user} failed to stop spectating (not spectating)", ConsoleColor.Yellow);
                return;
            }
            
            await toSpec.RemoveSpectator(user);
            XConsole.Log($"{user} stop spectating {toSpec}", ConsoleColor.Magenta);
        }
    }
}