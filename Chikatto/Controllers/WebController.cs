using System.Threading.Tasks;
using Chikatto.Objects;
using Microsoft.AspNetCore.Mvc;

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
        
        //TODO: /web/ 
    }
}