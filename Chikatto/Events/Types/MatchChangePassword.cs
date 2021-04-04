using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class MatchChangePassword
    {
        [Event(PacketType.OsuMatchChangePassword, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            if (user.Match.HostId != user.Id)
                return;

            user.Match.Password = reader.ReadString();
            await user.Match.Update();
        }
    }
}