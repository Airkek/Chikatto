﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Enums;
using Chikatto.Bancho.Objects;
using Chikatto.Bancho.Serialization;
using Chikatto.Constants;
using Chikatto.Objects;
using Chikatto.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Chikatto.Controllers
{
    public class BanchoController : Controller
    {
        [Route("/")]
        public async Task<IActionResult> Bancho(
            [FromHeader(Name = "osu-token")] string token,
            [FromHeader(Name = "User-Agent")] string userAgent
        )
        {
            if (Request.Method == "GET" || userAgent != "osu!")
                return Ok($"Running Chikatto v{Misc.Version}");

            Response.Headers["cho-protocol"] = "19";
            
            await using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms);
            ms.Position = 0;

            if (string.IsNullOrEmpty(token))
            {
#if DEBUG
                var sw = Stopwatch.StartNew();
#endif
                var req = Encoding.UTF8.GetString(ms.ToArray()).Split(new[] {"\n", "\r\n"}, StringSplitOptions.None);

                if (req.Length != 4)
                    return NotFound();

                Response.Headers["cho-token"] = "no-token";

                var u = await Auth.Login(req[0], req[1]);

                if (u.Id == -1)
                    return SendPackets(new[] { FastPackets.UserId(-1) });
                
                if (u.LastPong > new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() - 10)
                {
                    return SendPackets(new[]
                    {
                        FastPackets.UserId(-1),
                        FastPackets.Notification("User already logged in")
                    });
                }
                    
                
                token = RandomFabric.GenerateBanchoToken();
                Response.Headers["cho-token"] = token;

                var packets = new List<Packet>();
                
                packets.Add(FastPackets.UserId(u.Id));
                packets.Add(FastPackets.MainMenuIcon($"{Global.Config.LogoIngame}|{Global.Config.LogoClickUrl}"));
                packets.Add(FastPackets.Notification($"Welcome back!\r\nChikatto Build v{Misc.Version}"));
                packets.Add(await FastPackets.UserStats(u));
                packets.Add(FastPackets.UserPresence(u));
                packets.Add(FastPackets.BotPresence());
                packets.Add(FastPackets.BotStats());
                
                foreach (var (_, value) in Global.UserCache.Where(us => us.Key != u.Id))
                {
                    packets.Add(FastPackets.UserPresence(value));
                    packets.Add(await FastPackets.UserStats(value));
                }
                
                Global.TokenCache[token] = u.Id;
                Global.UserCache[u.Id] = u;
#if DEBUG
                Console.WriteLine($"{u} logged in (login took {sw.Elapsed.TotalMilliseconds}ms)");
#else
                Console.WriteLine($"{u} logged in");
#endif
                
                return SendPackets(packets);
            }

            if (!Global.TokenCache.ContainsKey(token))
            {
                var packets = new[]
                {
                    FastPackets.Notification("Server has restarted"), 
                    FastPackets.ServerRestart(0)
                };
                
                return SendPackets(packets);
            }

            var userId = Global.TokenCache[token];
            var user = Global.UserCache[userId];
            
            var osuPackets = Packets.GetPackets(ms.ToArray());

            foreach (var packet in osuPackets)
                await packet.Handle(user);

            if (Global.UserCache.ContainsKey(user.Id))
            {
                var res = user.WaitingPackets.ToArray();
            
                user.WaitingPackets.Clear();
                
                return SendPackets(res);
            }

            return SendPackets(Array.Empty<Packet>());
        }

        private FileContentResult SendPackets(IEnumerable<Packet> packets) =>
            File(Packets.GetBytes(packets), "application/octet-stream"); 
    }
}