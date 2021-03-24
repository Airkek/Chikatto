using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Chikatto.Controllers
{
    [Route("/web/")]
    public class WebController : Controller
    {
        [Route("check-updates.php")]
        public async Task<IActionResult> CheckUpdates() => Redirect("https://old.ppy.sh/web/check-updates.php" + Request.QueryString);

        [HttpPost("osu-session.php")]
        public async Task<IActionResult> OsuSession() => NotFound("NotImplemented"); // TODO: osu-session

        [Route("osu-getseasonal.php")]
        public async Task<IActionResult> GetSeasonalBgs() => Ok("[\"https://akatsuki.pw/static/flower.png\"]"); // TODO: seasonal bgs in config

        [Route("bancho_connect.php")]
        public async Task<IActionResult> BanchoConnect() => Ok("Chikatto");
        
        //TODO: /web/
    }
}