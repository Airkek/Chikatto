using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class MatchStart
    {
        [Event(PacketType.OsuMatchStart, false)]
        public static async Task Handle(PacketReader _, Presence user)
        {
            if(user.Id != user.Match.HostId)
                return;

            await user.Match.Start();
            await user.Match.Update();
        }
    }
}