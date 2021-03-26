using System;
using System.Collections.Generic;
using Chikatto.Database;
using Chikatto.Database.Models;

namespace Chikatto.Objects
{
    public static class Global
    {
        public static User Bot = new() {Id = 1, Name = "DenBai"};

        public static ConfigScheme Config;
        public static GulagDbContext Database;

        public static string DbConnectionString => $"server={Config.DatabaseHost};database={Config.DatabaseName};" +
                                                   $"user={Config.DatabaseUser};password={Config.DatabasePassword};";


        public static Dictionary<int, User> UserCache = new (); // <Id, User>
        public static Dictionary<string, int> IdCache = new(); // <SafeName, Id>
        public static Dictionary<string, int> TokenCache = new (); //<Token, Id>
        public static Dictionary<string, string> BCryptCache = new(); //<Plain, Hash>
    }
}