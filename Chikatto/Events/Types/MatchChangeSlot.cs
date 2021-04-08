using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class MatchChangeSlot
    {
        [Event(PacketType.OsuMatchChangeSlot, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            if(user.Match.InProgress)
                return;

            var index = reader.ReadInt32();
            
            if(index < 0 || index > 15)
                return;

            var slot = user.Match.Slots.ElementAt(index);
            
            if(slot.Status == SlotStatus.Locked || (slot.Status & SlotStatus.HasPlayer) != 0)
                return;
            
            var uSlot = user.Match.GetSlot(user.Id);

            slot.Status = uSlot.Status;
            slot.Mods = uSlot.Mods;
            slot.User = uSlot.User;
            slot.Team = uSlot.Team;
            
            uSlot.User = null;
            uSlot.Mods = Mods.NoMod;
            uSlot.Team = MatchTeam.Neutral;
            uSlot.Status = SlotStatus.Open;
            
            await user.Match.Update();
        }
    }
}