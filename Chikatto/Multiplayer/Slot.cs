using System.Threading.Tasks;
using Chikatto.Enums;
using Chikatto.Objects;

namespace Chikatto.Multiplayer
{
    public class Slot
    {
        public SlotStatus Status;
        public MatchTeam Team;
        public Mods Mods = Mods.NoMod;
        public Presence User;
        public int UserId => User.Id;

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