using System.Linq;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Constants;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class MatchTransferHost
    {
        [Event(PacketType.OsuMatchTransferHost, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            if(user.Match.InProgress || user.Match.HostId != user.Id)
                return;

            var index = reader.ReadInt32();
            
            if(index < 0 || index > 15)
                return;

            var slot = user.Match.Slots.ElementAt(index);
            
            if((slot.Status & SlotStatus.HasPlayer) == 0)
                return;

            user.Match.Host = slot.User;
            user.WaitingPackets.Enqueue(FastPackets.MatchTransferHost);
            await user.Match.Update();
        }
    }
}