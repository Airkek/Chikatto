using System.Collections.Concurrent;
using System.Reflection;
using Chikatto.Config;
using Chikatto.Database.Models;
using Chikatto.Multiplayer;
using Chikatto.Objects;

namespace Chikatto
{
    public static class Global
    {
        public static User Bot;
        public static byte BotCountry;

        public static ushort MatchId = 0;

        public static ConfigScheme Config;

        public static readonly ConcurrentStack<string> SubmittedScores = new(); // anti-duplicate scores (temp fix)

        public static readonly OnlineManager OnlineManager = new ();
        public static readonly BeatmapManager BeatmapManager = new ();
        
        public static readonly ConcurrentDictionary<string, Channel> Channels = new (); // <ChannelName, Channel>
        public static readonly ConcurrentDictionary<int, Match> Rooms = new (); // <Id, Match>

        public static readonly ConcurrentDictionary<string, string> BCryptCache = new (); //<Hash, Plain>

        public static readonly ConcurrentDictionary<string, PropertyInfo> StatsReflectionCache = new();
        
        public static PropertyInfo GetPropertyFromStatsCache(string name, string mode) // TODO: перенести бы это куда нить
        {
            var fullname = name + mode;
            
            if (Global.StatsReflectionCache.ContainsKey(fullname))
                return Global.StatsReflectionCache[fullname];

            var prop = typeof(Stats).GetProperty(fullname);

            Global.StatsReflectionCache[fullname] = prop;
            
            return prop;
        }
    }
}