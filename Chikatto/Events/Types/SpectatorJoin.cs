using System;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events.Types
{
    public class SpectatorJoin
    {
        [Event(PacketType.OsuStartSpectating, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            var id = reader.ReadInt32();
            var toSpec = Global.OnlineManager.GetById(id);

            if (toSpec is null)
            {
                XConsole.Log($"{user} failed to spectate <({id})> (offline)", ConsoleColor.Yellow);
                return;
            }

            await toSpec.AddSpectator(user);
            XConsole.Log($"{user} started spectating {toSpec}", ConsoleColor.Magenta);
        }
    }
}