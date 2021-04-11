using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class MatchChangeTeam
    {
        [Event(PacketType.OsuMatchChangeTeam, false)]
        public static async Task Handle(PacketReader _, Presence user)
        {
            if(user.Match.TeamType != MatchTeamType.TagTeamVS && user.Match.TeamType != MatchTeamType.TeamVS)
                return;

            var slot = user.Match.GetSlot(user.Id);
            
            if(slot.Status == SlotStatus.Playing)
                return;
            
            slot.Team = slot.Team == MatchTeam.Blue ? MatchTeam.Red : MatchTeam.Blue;
            await user.Match.Update();
        }
    }
}