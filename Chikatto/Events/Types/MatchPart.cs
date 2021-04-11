using System;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events.Types
{
    public class MatchPart
    {
        [Event(PacketType.OsuPartMatch, false)]
        public static async Task Handle(PacketReader _, Presence user)
        {
            if(user.Match is null)
                return;
            
            await user.Match.Leave(user);
            XConsole.Log($"{user} left from multiplayer room {user.Match}", ConsoleColor.Cyan);
        }
    }
}