using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class FriendRemove
    {
        [Event(PacketType.OsuFriendRemove)]
        public static Task Handle(PacketReader reader, Presence user)
        {
            var id = reader.ReadInt32();
            return user.RemoveFriend(id);
        }
    }
}