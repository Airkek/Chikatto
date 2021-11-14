using System;
using System.Threading.Tasks;
using Chikatto.Objects;
using Chikatto.Utils;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace Chikatto.Redis.ChannelHandlers
{
    public class Ban
    {
        [RedisHandler("peppy:ban")]
        public static async Task Handler(RedisChannel channel, RedisValue value)
        {
            if (!int.TryParse(value.ToString(), out var id))
                return;
            var user = await Presence.FromDatabase(id);

            if (user is null)
                return;

            await user.Ban(false); // should I restrict?
        }
    }
}