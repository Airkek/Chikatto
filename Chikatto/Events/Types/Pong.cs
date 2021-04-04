using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class Pong
    {
        [Event(PacketType.OsuPong)]
        public static async Task Handle(PacketReader reader, Presence user){}
    }
}