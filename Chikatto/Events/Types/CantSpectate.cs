using System;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events.Types
{
    public class CantSpectate
    {
        [Event(PacketType.OsuCantSpectate, false)]
        public static async Task Handle(PacketReader _, Presence user)
        {
            var toSpec = user.Spectating;

            if (toSpec is null)
            {
                XConsole.Log($"{user} sent cant spectate while not spectatinmg", ConsoleColor.Yellow);
                return;
            }

            var packet = await FastPackets.CantSpectate(user.Id);
            
            toSpec.WaitingPackets.Enqueue(packet);
            await toSpec.AddPacketToSpectators(packet);
        }
    }
}