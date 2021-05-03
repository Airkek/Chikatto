using System;
using StackExchange.Redis;

namespace Chikatto.Utils
{
    public static class Redis
    {
        public static void Init()
        {
            if(!Global.Config.Redis.Enabled)
                return;


            var connStr = $"{Global.Config.Redis.Host}:{Global.Config.Redis.Port}";

            if (!string.IsNullOrEmpty(Global.Config.Redis.Password))
                connStr += $",{Global.Config.Redis.Password}";
            
            connection = new Lazy<ConnectionMultiplexer>(() => 
                ConnectionMultiplexer.Connect(connStr));
        }
        
        private static Lazy<ConnectionMultiplexer> connection;
        public static ConnectionMultiplexer Connection => connection.Value;
    }
}