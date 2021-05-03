using System.Threading.Tasks;
using Chikatto.Objects;
using Microsoft.AspNetCore.Mvc;

namespace Chikatto.Controllers
{
    public class DirectController : Controller
    {
        [HttpGet]
        [Route("/d/{id}")]
        public async Task<IActionResult> Download(string id) =>
            Redirect($"{Global.Config.Cheesegull.DownloadMirror}{id.Replace("n", "")}");
    }
}