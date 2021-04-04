﻿using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class FriendAdd
    {
        [Event(PacketType.OsuFriendAdd)]
        public static Task Handle(PacketReader reader, Presence user)
        {
            var id = reader.ReadInt32();
            return user.AddFriend(id);
        }
    }
}