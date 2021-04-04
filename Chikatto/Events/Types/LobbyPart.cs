using System;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events.Types
{
    public class LobbyPart
    {
        [Event(PacketType.OsuPartLobby, false)]
        public static async Task Handle(PacketReader _, Presence user)
        {
            user.InLobby = false;
            await Global.Channels["#lobby"].RemoveUser(user);
            
            XConsole.Log($"{user} left from lobby", ConsoleColor.Cyan);
        }
    }
}