using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class StatusUpdate
    {
        [Event(PacketType.OsuRequestStatusUpdate)]
        public static async Task Handle(PacketReader _, Presence user)
        {
            user.WaitingPackets.Enqueue(await FastPackets.UserStats(user));
        }
    }
}