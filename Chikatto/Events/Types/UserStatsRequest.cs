using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Objects;

namespace Chikatto.Events.Types
{
    public class UserStatsRequest
    {
        [Event(PacketType.OsuUserStatsRequest)]
        public static async Task Handle(PacketReader reader, Presence user)
        {
            var players = reader.ReadInt32Array();
            
            foreach (var i in players)
            {
                if (i == Global.Bot.Id)
                {
                    user.WaitingPackets.Enqueue(await FastPackets.BotStats());
                    continue;
                }

                var us = Global.OnlineManager.GetById(i);
                if (us is not null)
                    user.WaitingPackets.Enqueue(await FastPackets.UserStats(us));
                else
                    user.WaitingPackets.Enqueue(await FastPackets.Logout(i));
            }
        }
    }
}