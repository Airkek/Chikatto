using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Constants;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class MatchCompleted
    {
        [Event(PacketType.OsuMatchComplete, false)]
        public static async Task Handle(PacketReader _, Presence user)
        {
            var match = user.Match;
            var slot = match.GetSlot(user.Id);

            slot.Status = SlotStatus.Complete;
            
            if(match.Slots.Any(x => x.Status == SlotStatus.Playing))
                return;

            await match.Unready();
            await match.AddPacketsToAllPlayers(FastPackets.MatchComplete);
            await match.Update();
        }
    }
}