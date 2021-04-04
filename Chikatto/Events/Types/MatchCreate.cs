using System;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Multiplayer;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events.Types
{
    public class MatchCreate
    {
        [Event(PacketType.OsuCreateMatch, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            if (user.Match is not null)
                await user.Match.Leave(user);

            var match = reader.ReadBanchoObject<Match>();

            Global.Rooms[match.Id] = match;

            await match.Join(user, match.Password);
            await match.Update();

            await Global.OnlineManager.AddPacketToAllUsers(await FastPackets.NewMatch(match.Foreign()));

            XConsole.Log($"{user} created multiplayer room {match}", ConsoleColor.Green);
        }
    }
}