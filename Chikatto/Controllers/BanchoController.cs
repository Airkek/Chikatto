using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using HOPEless.Bancho;
using HOPEless.Bancho.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;
using osu.Shared.Serialization;

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
                return Ok("Running Chikatto");

            var packets = new List<BanchoPacket>();
            
            packets.Add(new BanchoPacket(PacketType.ServerNotification, new BanchoString("Welcome back to Chikatto!")));

            if (string.IsNullOrEmpty(token))
            {
                //TODO: auth
                //Response.Headers["cho-token"] = "chikatto-1337"; //TODO: implement tokens
                
                return Packets(packets);
            }
            
            // TODO: bancho packets
            return Packets(packets);
        }

        private FileContentResult Packets(IEnumerable<BanchoPacket> packets) => File(BanchoSerializer.Serialize(packets), "application/octet-stream");
    }
}