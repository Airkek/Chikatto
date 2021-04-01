using System.Collections.Concurrent;
using Chikatto.Database;
using Chikatto.Database.Models;

namespace Chikatto.Objects
{
    public static class Global
    {
        public static User Bot;
        public static byte BotCountry;

        public static ConfigScheme Config;

        public static OnlineManager OnlineManager = new ();
        public static ConcurrentDictionary<string, Channel> Channels = new (); // <ChannelName, Channel>

        public static ConcurrentDictionary<string, string> BCryptCache = new (); //<Plain, Hash>
    }
}