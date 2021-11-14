using System.Threading.Tasks;
using Chikatto.Objects;
using Chikatto.Utils;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace Chikatto.Redis.ChannelHandlers
{
    public class Utils
    {
        public static int GetId(RedisChannel channel, JObject json)
        {
            if (json.ContainsKey("userID") && int.TryParse((string) json.GetValue("id"), out var id)) 
                return id;
            
            XConsole.Log($"Error in ({channel}) / userID must be integer: {json}");
            return -1;
        }

        public static async Task<Presence> GetUser(RedisChannel channel, JObject json)
        {
            var id = GetId(channel, json);

            return id == -1 ? null : await Presence.FromDatabase(id);
        }
    }
}