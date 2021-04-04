using System;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;
using Chikatto.Utils;

namespace Chikatto.Events.Types
{
    public class Logout
    {
        [Event(PacketType.OsuLogout)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            user.LastPong = 0;
            
            foreach (var (_, c) in Global.Channels)
                await c.RemoveUser(user);

            if (user.Match is not null)
                await user.Match.Leave(user);
            
            await Global.OnlineManager.Remove(user);
            await Global.OnlineManager.AddPacketToAllUsers(await FastPackets.Logout(user.Id));
            
            XConsole.Log($"{user} logged out", ConsoleColor.Green);
        }
    }
}