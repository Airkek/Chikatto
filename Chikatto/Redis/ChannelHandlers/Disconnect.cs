using System;
using System.Threading.Tasks;
using Chikatto.Utils;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace Chikatto.Redis.ChannelHandlers
{
    public class Disconnect
    {
        [RedisHandler("peppy:disconnect")]
        public static async Task Handler(RedisChannel channel, RedisValue value)
        {
            var json = JObject.Parse(value.ToString());

            var user = await Utils.GetUser(channel, json);

            if (user is null)
                return;

            if (user.Online)
            {
                var reason = json.GetValue("reason");
                await user.Notify($"You was kicked for {reason}");
                await Global.OnlineManager.Remove(user);
                XConsole.Log($"{user} was kicked for {reason}", fore: ConsoleColor.Yellow);
            }
        }
    }
}