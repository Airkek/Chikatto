using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class MatchSlotLock
    {
        [Event(PacketType.OsuMatchSlotLock, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            if(user.Match.InProgress || user.Match.HostId != user.Id)
                return;

            var index = reader.ReadInt32();
            
            if(index is < 0 or > 15)
                return;

            var slot = user.Match.Slots.ElementAt(index);

            await slot.Toggle();
            await user.Match.Update();
        }
    }
}