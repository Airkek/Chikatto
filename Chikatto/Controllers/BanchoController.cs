using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Chikatto.Bancho;
using Chikatto.Bancho.Serialization;
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
                return Ok("Running Chikatto");

            var packets = new List<Packet>();
            
            Response.Headers["cho-protocol"] = "19";

            if (string.IsNullOrEmpty(token))
            {
                //TODO: auth
                Response.Headers["cho-token"] = "chikatto-1337";

                return File(new byte[] {  }, "application/octet-stream");

                //packets.Add(new BanchoPacket(PacketType.ServerLoginReply, new BanchoInt(3)));
                //packets.Add(new BanchoPacket(PacketType.ServerNotification, new BanchoString("Hey! Welcome back to Chikatto")));

                //return Packets(packets);
            }

            await using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms);
            ms.Position = 0;

            var osuPackets = Packets.Read(ms.ToArray());

            foreach (var packet in osuPackets)
                Console.WriteLine(packet);
            
            // TODO: bancho packets
            return SendPackets(packets);
        }

        private FileContentResult SendPackets(IEnumerable<Packet> packets) =>
            File(Array.Empty<byte>(), "application/octet-stream"); //File(PacketWriter.Serialize(packets), "application/octet-stream");
    }
}