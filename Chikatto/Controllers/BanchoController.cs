using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Serialization;
using Chikatto.Constants;
using Chikatto.Objects;
using Chikatto.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Chikatto.Controllers
{
    public class BanchoController : Controller
    {
        public OkObjectResult Default() => Ok($"Running Chikatto v{Misc.Version}");
        
        [Route("/")]
        public async Task<IActionResult> Bancho(
            [FromHeader(Name = "osu-token")] string token,
            [FromHeader(Name = "User-Agent")] string userAgent
        )
        {
            if (Request.Method == "GET" || userAgent != "osu!")
                return Default();

            Response.Headers["cho-protocol"] = Misc.BanchoProtocolVersion.ToString();
            
            await using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms);
            ms.Position = 0;

            if (string.IsNullOrEmpty(token))
            {
                var sw = Stopwatch.StartNew();

                var req = Encoding.UTF8.GetString(ms.ToArray()).Split(new[] {"\n", "\r\n"}, StringSplitOptions.None);

                if (req.Length != 4)
                    return NotFound();

                Response.Headers["cho-token"] = "chikatto::no-token";

                var u = await Auth.Login(req[0], req[1]);
                var clientData = req[2].Split(":");

                if (u is null)
                    return SendPackets(new[] { await FastPackets.UserId(-1) });
                
                if (u.LastPong > new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() - 10)
                {
                    return SendPackets(new[]
                    {
                        await FastPackets.UserId(-1),
                        await FastPackets.Notification("User already logged in")
                    });
                }
                
                token = Auth.CreateBanchoToken(u.Id, clientData);
                Response.Headers["cho-token"] = token;

                u.Token = token;
                await Global.OnlineManager.AddUser(u);
                var users = await Global.OnlineManager.GetOnlineUsers();

                var packets = new List<Packet>
                {
                    await FastPackets.ProtocolVersion(),
                    await FastPackets.UserId(u.Id),
                    await FastPackets.MainMenuIcon(Global.Config.LogoIngame, Global.Config.LogoClickUrl),
                    await FastPackets.Notification($"Welcome back!\r\nChikatto Build v{Misc.Version}"),
                    await FastPackets.BotPresence(),
                    await FastPackets.BotStats()
                };


                foreach (var us in users)
                {
                    packets.Add(await FastPackets.UserPresence(us));
                    packets.Add(await FastPackets.UserStats(us));
                }

                var channels = Global.Channels.Select(x => x.Value).Where(x => (x.Read & u.User.Privileges) == x.Read);
                
                foreach (var channel in channels)
                {
                    if((channel.Read & u.User.Privileges) != channel.Read)
                        continue;

                    if (channel.Default)
                    {
                        packets.Add(await FastPackets.ChannelAutoJoin(channel));
                        await channel.JoinUser(u);
                    }
                    else 
                        u.WaitingPackets.Enqueue(await channel.GetInfoPacket());
                }

                packets.Add(FastPackets.ChannelInfoEnd);
                
                XConsole.Log($"{u} logged in (login took {sw.Elapsed.TotalMilliseconds}ms)", ConsoleColor.Green);
                
                return SendPackets(packets);
            }

            if (!token.StartsWith("chikatto:"))
                return Default();
            
            var user = Global.OnlineManager.GetByToken(token);

            if (user is null)
            {
                var packets = new[]
                {
                    await FastPackets.Notification("Server has restarted"), 
                    await FastPackets.ServerRestart(0)
                };
                
                return SendPackets(packets);
            }
            
            user.LastPong = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

            var osuPackets = Packets.GetPackets(ms.ToArray());

            foreach (var packet in osuPackets)
                await packet.Handle(user);

            var res = new List<Packet>();

            while (!user.WaitingPackets.IsEmpty)
            {
                if(user.WaitingPackets.TryDequeue(out var packet))
                    res.Add(packet);
            }

            return SendPackets(res);
        }

        private FileContentResult SendPackets(IEnumerable<Packet> packets) =>
            File(Packets.GetBytes(packets), "application/octet-stream"); 
    }
}