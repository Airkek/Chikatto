using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class MatchChangeMods
    {
        [Event(PacketType.OsuMatchChangeMods, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            if(user.Match.InProgress)
                return;

            var mods = (Mods) reader.ReadInt32();

            var match = user.Match;
            if (match.FreeMod)
            {
                if (match.HostId == user.Id)
                    match.Mods = mods & Mods.SpeedAltering;

                var slot = match.GetSlot(user.Id);
                slot.Mods = mods & ~Mods.SpeedAltering;
            }
            else if (match.HostId == user.Id)
            {
                match.Mods = mods;
            }
            else
                return;

            await match.Update();
        }
    }
}