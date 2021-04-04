using System;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events.Types
{
    public class LobbyJoin
    {
        [Event(PacketType.OsuJoinLobby, false)]
        public static async Task Handle(PacketReader _, Presence user)
        {
            user.InLobby = true;
            await Global.Channels["#lobby"].JoinUser(user);
            
            foreach (var (_, match) in Global.Rooms)
                user.WaitingPackets.Enqueue(await FastPackets.NewMatch(match.Foreign()));

            XConsole.Log($"{user} joined to lobby", ConsoleColor.Cyan);
        }
    }
}