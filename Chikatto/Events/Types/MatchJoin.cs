using System;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events.Types
{
    public class MatchJoin
    {
        [Event(PacketType.OsuJoinMatch, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            if (user.Silenced)
                return;
            
            if (user.Match is not null)
                await user.Match.Leave(user);

            var id = reader.ReadInt32();
            
            if (!Global.Rooms.ContainsKey(id))
            {
                user.WaitingPackets.Enqueue(FastPackets.MatchJoinFail);
                return;
            }

            var password = reader.ReadString();
            var match = Global.Rooms[id];

            await match.Join(user, password);
            
            XConsole.Log($"{user} joined to multiplayer room {match}", ConsoleColor.Cyan);
        }
    }
}