﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chikatto.Database.Models;
using Chikatto.Objects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Chikatto.Utils
{
    public static class Auth
    {
        public static async Task<Presence> Login(string name, string pwMd5)
        {
            var safe = GetSafeName(name);
            
            if(safe == Global.Bot.SafeName)
                return null;
            
            var user = await Presence.FromDatabase(GetSafeName(name));

            if (user is null)
                return null;

            if (Global.BCryptCache.ContainsKey(user.User.Password))
                return Global.BCryptCache[user.User.Password] == pwMd5 ? user : null;
                
            var valid = BCrypt.Net.BCrypt.Verify(pwMd5, user.User.Password);

            if (!valid) 
                return null;
            
            Global.BCryptCache[user.User.Password] = pwMd5;
            return user;
        }

        public static string CreateBanchoToken(int userId, string[] clientData)
        {
            var mac = clientData[1].ToLower().Split(".")[0];
            return string.Concat(
                "chikatto:",
                userId,
                ":c",
                mac.Substring(0, Math.Min(mac.Length, 5)), // first part of MacAddress (limit by 5 chars)
                "o",
                clientData[0].Substring(0, 3), // first 3 chars of md5(osu!.exe)
                "f",
                clientData[2].Substring(0, 5), // first 5 chars of Adapters
                "f",
                clientData[3].Substring(0, 5), // first 5 chars of UninstallId
                "e",
                clientData[4].Substring(0, 5), // first 5 chars of DiskSig
                "e"
            );
        }

        public static string GetSafeName(string name) => name.ToLower().Replace(" ", "_");
    }
}