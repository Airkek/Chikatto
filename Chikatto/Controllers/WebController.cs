using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chikatto.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Chikatto.Enums;
using Chikatto.Objects;
using Chikatto.Utils;
using Chikatto.Utils.Cheesegull;

namespace Chikatto.Controllers
{
    [Route("/web/")]
    public class WebController : Controller
    {
        [Route("check-updates.php")]
        public async Task<IActionResult> CheckUpdates() => Redirect("https://old.ppy.sh/web/check-updates.php" + Request.QueryString);

        [HttpPost("osu-session.php")]
        public async Task<IActionResult> OsuSession() => NotFound("NotImplemented");

        [Route("osu-getseasonal.php")]
        public async Task<IActionResult> GetSeasonalBgs() => Ok(Global.Config.SeasonalBgsJson);

        [Route("bancho_connect.php")]
        public async Task<IActionResult> BanchoConnect() => Ok("Chikatto");

        [HttpGet("osu-search.php")]
        public async Task<IActionResult> DirectSearch()
        {
            if (!await CheckAuthorization() || !int.TryParse(Request.Query["p"], out var offset)
                                            || !int.TryParse(Request.Query["m"], out var mode) 
                                            || !int.TryParse(Request.Query["r"], out var _status))
            {
                return BadRequest();
            }

            var status = (DirectRankedStatus) _status;

            var query = string.Empty;

            if (Request.Query["q"] != "Newest" && Request.Query["q"] != "Top+Rated" &&
                Request.Query["q"] != "Most+Played")
            {
                query = Request.Query["q"];
            }

            var res = await CheesegullHelper.Search(offset, query, mode, status);

            var output = new List<string>
            {
                res.Length == 100 ? "101" : res.Length.ToString()
            };

            foreach (var set in res)
            {
                if(set.ChildrenBeatmaps is null || set.ChildrenBeatmaps.Count == 0)
                    continue;
                
                set.ChildrenBeatmaps.Sort((x, y) => x.DifficultyRating.CompareTo(y.DifficultyRating));
                output.Add($"{set.SetId}.osz|{set.Artist}|{set.Title}|{set.Creator}|{set.RankedStatus}|10.0|{set.LastUpdate}|{set.SetId}|0|0|0|0|0|" + string.Join(",", set.ChildrenBeatmaps.Select(x => 
                    $"[{x.DifficultyRating:n2}⭐] {x.DiffName} {{CS{x.CS} OD{x.OD} AR{x.AR} HP{x.HP}}}@{(int) x.Mode}")));
            }
            
            return Ok(string.Join("\r\n", output));
        }

        [HttpGet("osu-search-set.php")]
        public async Task<IActionResult> SetSearch()
        {
            if (!await CheckAuthorization())
                return BadRequest();


            Utils.Cheesegull.Models.BeatmapSet set = null;
            
            if (Request.Query.ContainsKey("s"))
            {
                if (!int.TryParse(Request.Query["s"], out var s))
                    return BadRequest();
                
                set = await CheesegullHelper.GetSet(s);
            }
            else if (Request.Query.ContainsKey("b"))
            {
                if (!int.TryParse(Request.Query["b"], out var b))
                    return BadRequest();
                
                var map = await CheesegullHelper.GetMap(b);

                set = await CheesegullHelper.GetSet(map.ParentSetID);
            }

            if (set is null)
                return NotFound();
            
            return Ok($"{set.SetId}.osz|{set.Artist}|{set.Title}|{set.Creator}|{set.RankedStatus}|10.0|0|{set.SetId}|0|0|0|0|0");
        }

        //TODO: /web/ 

        public async Task<Presence> GetPresence()
        {
            var user = Auth.GetSafeName(Request.Query["u"]);
            var pwMd5 = (string) Request.Query["h"];

            var presence = await Auth.Login(user, pwMd5);

            if (presence is null || !presence.Online)
                XConsole.Log($"{presence?.ToString() ?? user} requested {Request.Path} while not online", ConsoleColor.Yellow);

            return presence;
        }

        public async Task<bool> CheckAuthorization() => await GetPresence() is not null;
    }
}