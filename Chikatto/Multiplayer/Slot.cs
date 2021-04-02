using Chikatto.Constants;
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
    }
}