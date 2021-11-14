using System;
using System.Threading.Tasks;
using Chikatto.Objects;
using Chikatto.Utils;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace Chikatto.Redis.ChannelHandlers
{
    public class Silence
    {
        [RedisHandler("peppy:silence")]
        public static async Task Handler(RedisChannel channel, RedisValue value)
        {
            if (!int.TryParse(value.ToString(), out var id))
                return;
            var user = await Presence.FromDatabase(id);

            if (user is null)
                return;

            await user.Mute(10 * 60 * 60 * 24 * 365, "redis peppy:silence"); //на 10 лет ;cc
        }
    }
}