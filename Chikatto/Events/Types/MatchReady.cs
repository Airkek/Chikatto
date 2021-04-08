﻿using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class MatchReady
    {
        [Event(PacketType.OsuMatchReady, false)]
        public static async Task Handle(PacketReader _, Presence user)
        {
            await user.Match.UpdateUserStatus(user, SlotStatus.Ready);
        }
    }
}