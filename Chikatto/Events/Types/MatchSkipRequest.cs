using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class MatchSkipRequest
    {
        [Event(PacketType.OsuMatchSkipRequest, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            var match = user.Match;

            var uSlot = match.GetSlot(user.Id);
            uSlot.Skipped = true;

            var packet = await FastPackets.MatchPlayerSkipped(user.Id);
            await match.AddPacketsToAllPlayers(packet);

            if (!match.Slots.Any(slot => slot.Status == SlotStatus.Playing && !slot.Skipped))
                await match.AddPacketsToAllPlayers(FastPackets.MatchSkip);
        }
    }
}