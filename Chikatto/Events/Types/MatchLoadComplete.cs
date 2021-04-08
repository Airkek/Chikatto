using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class MatchLoadComplete
    {
        [Event(PacketType.OsuMatchLoadComplete, false)]
        public static async Task Handle(PacketReader _, Presence user)
        {
            var match = user.Match;
            var slot = match.GetSlot(user.Id);
            
            if(slot.Status != SlotStatus.Playing)
                return;

            if (--match.NeedLoad <= 0)
            {
                await match.AddPacketsToAllPlayers(FastPackets.MatchAllPlayersLoaded);
                match.NeedLoad = 0;
            }
        }
    }
}