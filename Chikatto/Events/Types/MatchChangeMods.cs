using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Enums;
using Chikatto.Objects;
using osu.Game.Beatmaps.Legacy;

namespace Chikatto.Events.Types
{
    public class MatchChangeMods
    {
        [Event(PacketType.OsuMatchChangeMods, false)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            if(user.Match.InProgress)
                return;

            var mods = (LegacyMods) reader.ReadInt32();

            var match = user.Match;
            if (match.FreeMod)
            {
                if (match.HostId == user.Id)
                    match.Mods = mods & (LegacyMods.DoubleTime | LegacyMods.HalfTime);

                var slot = match.GetSlot(user.Id);
                slot.Mods = mods & ~(LegacyMods.DoubleTime | LegacyMods.HalfTime);
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