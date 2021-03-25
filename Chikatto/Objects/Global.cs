using System;
using System.Collections.Generic;

namespace Chikatto.Objects
{
    public static class Global
    {
        public static User Bot = new() {Id = 1, Name = "DenBai"};
        
        public static Dictionary<string, int> Test = new() // Test :D <SafeName, Id>
        {
            ["cookiezi"] = 3,
            ["peppy"] = 4,
            ["firedigger"] = 5,
        }; 
        
        public static Dictionary<int, User> UserCache = new()
        {
            [1] = Bot
        }; //<Id, User>
        public static Dictionary<string, int> TokenCache = new(); //<Token, Id>
    }
}