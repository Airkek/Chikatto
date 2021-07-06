using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Chikatto.Controllers
{
    public class ScreenshotController : Controller
    {
        [HttpGet]
        [Route("/ss/{file}")]
        public async Task<IActionResult> Avatar(string file)
        {
            var path = Path.Combine(".data", "screenshots", file);

            if (!System.IO.File.Exists(path))
                return NotFound("Screenshot not found");
            
            var suffix = file.Split('.')[1];
            var ext = suffix == "jpg" ? "jpeg" : suffix;
            var data = await System.IO.File.ReadAllBytesAsync(path);

            return File(data, $"image/{ext}");
        }
    }
}