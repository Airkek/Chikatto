using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;
using Microsoft.EntityFrameworkCore.Internal;

namespace Chikatto.Events.Types
{
    public class MatchScoreUpdate
    {
        [Event(PacketType.OsuMatchScoreUpdate, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            if (!user.Match.InProgress)
                return;

            var match = user.Match;

            var bytes = reader.Dump().Data;
            var index = match.Slots.Select(x => x.UserId).IndexOf(user.Id);

            bytes[4] = (byte) index;

            await match.AddPacketsToSpecificPlayers(await FastPackets.MatchScoreUpdate(bytes.ToArray()));
        }
    }
}