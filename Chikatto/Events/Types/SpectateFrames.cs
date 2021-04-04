using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class SpectateFrames
    {
        [Event(PacketType.OsuSpectateFrames, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            var frames = reader.Dump().Data;
            await user.AddPacketToSpectators(await FastPackets.SpectateFrames(frames));
        }
    }
}