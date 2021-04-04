using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class MatchPlayerFail
    {
        [Event(PacketType.OsuMatchFailed, false)]
        public static async Task Handle(PacketReader _, Presence user)
        {
            if (!user.Match.InProgress)
                return;

            var match = user.Match;
            var index = match.Slots.Select(x => x.UserId).ToList().IndexOf(user.Id);

            await match.AddPacketsToSpecificPlayers(await FastPackets.MatchPlayerFailed(index));
        }
    }
}