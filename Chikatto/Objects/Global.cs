using System;
using System.Collections.Generic;
using Chikatto.Database;
using Chikatto.Database.Models;

namespace Chikatto.Objects
{
    public static class Global
    {
        public static User Bot;

        public static ConfigScheme Config;
        public static GulagDbContext Database;

        public static string DbConnectionString => $"server={Config.DatabaseHost};database={Config.DatabaseName};" +
                                                   $"user={Config.DatabaseUser};password={Config.DatabasePassword};";

        public static OnlineManager Manager = new();

        public static Dictionary<string, string> BCryptCache = new(); //<Plain, Hash>
    }
}