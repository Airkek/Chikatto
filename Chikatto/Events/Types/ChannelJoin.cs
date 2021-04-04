using System;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events.Types
{
    public class ChannelJoin
    {
        [Event(PacketType.OsuChannelJoin)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            var channel = reader.ReadString();
            
            if (!Global.Channels.ContainsKey(channel) || channel == "#lobby")
                return;

            var c = Global.Channels[channel];

            if (c.IsLobby && !user.InLobby)
                return;
            
            await c.JoinUser(user);
            
            XConsole.Log($"{user} joined {c}", ConsoleColor.Cyan);
        }
    }
}