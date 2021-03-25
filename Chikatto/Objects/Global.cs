using System;
using System.Collections.Generic;

namespace Chikatto.Objects
{
    public static class Global
    {
        public static Dictionary<string, int> Test = new() // Test :D <SafeName, Id>
        {
            ["cookiezi"] = 3,
            ["peppy"] = 4,
            ["firedigger"] = 5,
        }; 
        
        public static Dictionary<int, User> UserCache = new(); //<Id, User>
        public static Dictionary<string, int> TokenCache = new(); //<Token, Id>
    }
}