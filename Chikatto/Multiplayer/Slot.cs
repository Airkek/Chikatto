using System.Threading.Tasks;
using Chikatto.Enums;
using Chikatto.Objects;
using osu.Game.Beatmaps.Legacy;

namespace Chikatto.Multiplayer
{
    public class Slot
    {
        public SlotStatus Status;
        public MatchTeam Team;
        public LegacyMods Mods = LegacyMods.None;
        public Presence User;
        public int UserId => User?.Id ?? -1;

        public bool Skipped = false;

        public async Task Toggle()
        {
            if (Status == SlotStatus.Locked)
            {
                Status = SlotStatus.Open;
                return;
            }
            
            if ((Status & SlotStatus.HasPlayer) != 0)
                await User.Match.Leave(User); 
                
            Status = SlotStatus.Locked;
        }
    }
}