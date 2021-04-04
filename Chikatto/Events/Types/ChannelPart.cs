using System;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events.Types
{
    public class ChannelPart
    {
        [Event(PacketType.OsuChannelPart)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            var channel = reader.ReadString();

            if (!Global.Channels.ContainsKey(channel) || channel == "#lobby")
                return;

            var c = Global.Channels[channel];

            await c.RemoveUser(user);

            XConsole.Log($"{user} left from {channel}", ConsoleColor.Cyan);
        }
    }
}