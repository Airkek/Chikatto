using System.Threading.Tasks;
using Chikatto.Database.Models;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace Chikatto.Redis.ChannelHandlers
{
    public class Notification
    {
        [RedisHandler("peppy:notification")]
        public static async Task Handler(RedisChannel channel, RedisValue value)
        {
            var json = JObject.Parse(value.ToString());

            var user = await Utils.GetUser(channel, json);
            
            if(user is null)
                return;

            if (user.Online)
                await user.Notify(json.GetValue("message").ToString());
        }
    }
}