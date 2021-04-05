using System.Collections.Concurrent;
using Chikatto.Database.Models;
using Chikatto.Multiplayer;

namespace Chikatto.Objects
{
    public static class Global
    {
        public static User Bot;
        public static byte BotCountry;

        public static ushort MatchId = 0;

        public static ConfigScheme Config;

        public static readonly OnlineManager OnlineManager = new ();
        public static readonly ConcurrentDictionary<string, Channel> Channels = new (); // <ChannelName, Channel>
        public static readonly ConcurrentDictionary<int, Match> Rooms = new (); // <Id, Match>

        public static readonly ConcurrentDictionary<string, string> BCryptCache = new (); //<Hash, plain>
    }
}