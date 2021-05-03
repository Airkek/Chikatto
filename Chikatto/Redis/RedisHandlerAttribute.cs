using System;
using StackExchange.Redis;

namespace Chikatto.Redis
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RedisHandlerAttribute : Attribute
    {
        public RedisChannel Channel;
        
        public RedisHandlerAttribute(RedisChannel channel)
        {
            Channel = channel;
        }

        public RedisHandlerAttribute(string channel)
        {
            Channel = new RedisChannel(channel, RedisChannel.PatternMode.Auto);
        }
    }
}