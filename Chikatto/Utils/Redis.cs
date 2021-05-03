using System;
using System.Threading.Tasks;
using Chikatto.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StackExchange.Redis;

namespace Chikatto.Utils
{
    public static class Redis
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