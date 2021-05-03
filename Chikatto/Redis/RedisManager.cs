using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Chikatto.ChatCommands;
using Chikatto.Constants;
using Microsoft.EntityFrameworkCore.Query.Internal;
using StackExchange.Redis;

namespace Chikatto.Redis
{
    public static class RedisManager
    {
        public static void Init()
        {
            if(!Global.Config.Redis.Enabled)
                return;


            var connStr = $"{Global.Config.Redis.Host}:{Global.Config.Redis.Port},defaultDatabase={Global.Config.Redis.DatabaseIndex}";

            if (!string.IsNullOrEmpty(Global.Config.Redis.Password))
                connStr += $",{Global.Config.Redis.Password}";
            
            connection = new Lazy<ConnectionMultiplexer>(() => 
                ConnectionMultiplexer.Connect(connStr));

            var cache = Connection.GetDatabase();

            cache.StringSet("ripple:online_users", "0");
            cache.StringSet("peppy:version", Misc.Version);
            cache.ScriptEvaluate(flush_script, new RedisKey []{ "peppy:*" });
            cache.ScriptEvaluate(flush_script, new RedisKey []{ "peppy:sessions:*" });
            cache.StringSet("peppy:version", Misc.Version);

            var methods = Assembly.GetEntryAssembly().GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(RedisHandlerAttribute), false).Length > 0);

            var subscriber = Connection.GetSubscriber();

            foreach (var method in methods)
            {
                var info = method.GetCustomAttributes(typeof(RedisHandlerAttribute), false)[0] as RedisHandlerAttribute;
                var handler = (Action<RedisChannel, RedisValue>) Delegate.CreateDelegate(typeof(Action<RedisChannel, RedisValue>), method);

                subscriber.Subscribe(info.Channel, handler);
                
                /*
                 * TODO:
                 * peppy:ban
                 * peppy:silence //TODO: silence
                 * peppy:update_cached_stats
                 * peppy:change_username
                 */
            }
        }
        
        private static Lazy<ConnectionMultiplexer> connection;
        public static ConnectionMultiplexer Connection => connection.Value;

        public static async Task Set(string key, string value)
        {
            var cache = Connection.GetDatabase();

            await cache.StringSetAsync(key, value);
        }


        private const string flush_script = @"local matches = redis.call('KEYS', KEYS[1])

local result = 0
for _,key in ipairs(matches) do
    result = result + redis.call('DEL', key)
end

return result";
    }
}