using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class ChangeAction
    {
        [Event(PacketType.OsuChangeAction)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            user.Status = reader.ReadBanchoObject<BanchoUserStatus>();
        }
    }
}