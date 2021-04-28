using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Chikatto.Bancho;
using Chikatto.Bancho.Serialization;
using Chikatto.Constants;
using Chikatto.Database;
using Chikatto.Events;
using Chikatto.Utils;
using Chikatto.Enums;

namespace Chikatto.Controllers
{
    public class BanchoController : Controller
    {
        public OkObjectResult Default()
        {
            var defStr = $"Running Chikatto v{Misc.Version}\r\n";

            defStr += $"Online players: {Global.OnlineManager.Online}\r\n";
            defStr += $"Multiplayer rooms: {Global.Rooms.Count}\r\n";
            
            return Ok(defStr);
        }
        
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
                {
                    return SendPackets(new[]
                    {
                        await FastPackets.UserId(-1) // bad password
                    });
                }

                if (u.LastPong > new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() - 10)
                {
                    return SendPackets(new[]
                    {
                        await FastPackets.UserId(-1), // bad password
                        await FastPackets.Notification("User already logged in")
                    });
                }
                
                u.LastPong = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

                if (u.Spectating is not null)
                    await u.Spectating.RemoveSpectator(u);
                
                if(u.Match is not null)
                    await u.Match.Leave(u);

                foreach (var (_, us) in u.Spectators)
                {
                    await u.RemoveSpectator(us);
                }

                var packets = new List<Packet>
                {
                    await FastPackets.ProtocolVersion(),
                    await FastPackets.UserId(u.Id),
                    await FastPackets.MainMenuIcon(Global.Config.LogoIngame, Global.Config.LogoClickUrl),
                    await FastPackets.Notification($"Welcome back!\r\nChikatto Build v{Misc.Version}"),
                    await FastPackets.BanchoPrivileges(await u.GetBanchoPermissions() | BanchoPermissions.Supporter),
                    await FastPackets.FriendList(u.Friends.Select(x => x.Key).ToList()),
                    await FastPackets.BotPresence(),
                };

                if (u.Restricted) 
                {
                    if ((u.User.Privileges & Privileges.Restricted) != 0) // account restricted
                    {
                        packets.Add(FastPackets.AccountRestricted);
                        await u.Notify("Your account is currently in restricted mode");
                    }
                    else // account banned
                    {
                        return SendPackets(new[]
                        {
                            await FastPackets.UserId(-3) // ban client
                        });
                    }
                }
                else if ((u.User.Privileges & Privileges.PendingVerification) != 0) // user just registered
                {
                    u.User.Privileges = (u.User.Privileges & ~Privileges.PendingVerification) | Privileges.Public | Privileges.Normal;
                    await Db.Execute("UPDATE users SET privileges = @priv WHERE id = @id", new { priv = u.User.Privileges, id = u.User.Id });
                    await u.SendMessage(
                        $"Welcome to our server. \r\nType {Global.Config.CommandPrefix}help to see list of available commands.",
                        Global.Bot);
                }

                if (u.Stats.Country.ToUpper() == "XX")
                {
                    var ip = (string) Request.Headers["X-Real-IP"];
                    var country = (await IpApi.FetchLocation(ip)).ToUpper();
                    u.Stats.Country = country;
                    u.CountryCode = Misc.CountryCodes.ContainsKey(country)
                        ? Misc.CountryCodes[u.Stats.Country.ToUpper()]
                        : (byte) 0;

                    await Db.Execute("UPDATE users_stats WHERE id = @id SET country = @country",
                        new {id = u.Id, country});
                    await Db.Execute("UPDATE rx_stats WHERE id = @id SET country = @country",
                        new {id = u.Id, country});
                }

                token = Auth.CreateBanchoToken(u.Id, clientData);
                Response.Headers["cho-token"] = token;

                u.Token = token;
                u.Online = true;
                await Global.OnlineManager.AddUser(u);
                var users = await Global.OnlineManager.GetOnlineUsers();

                foreach (var us in users)
                    packets.Add(await FastPackets.UserPresence(us));

                var channels = Global.Channels.Select(x => x.Value).Where(x => (x.Read & u.User.Privileges) != 0);

                foreach (var channel in channels)
                {
                    if((channel.Read & u.User.Privileges) != channel.Read || channel.IsTemp || channel.IsLobby)
                        continue;
                    
                    if (channel.Name == "#osu") 
                        await channel.JoinUser(u);
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
                return SendPackets(new[]
                {
                    await FastPackets.Notification("Server has restarted"),
                    await FastPackets.ServerRestart(0)
                });
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