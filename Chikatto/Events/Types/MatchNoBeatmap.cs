﻿using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Constants;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class MatchNoBeatmap
    {
        [Event(PacketType.OsuMatchNoBeatmap, false)]
        public static async Task Handle(PacketReader _, Presence user)
        {
            await user.Match.UpdateUserStatus(user, SlotStatus.NoMap);
        }
    }
}