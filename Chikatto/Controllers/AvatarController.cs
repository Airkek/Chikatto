using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Chikatto.Controllers
{
    public class AvatarController : Controller
    {
        private static string[] AvailableSuffixes = { "png", "jpg", "gif"};
        [Route("/{id:int}")]
        public async Task<IActionResult> Avatar(int id)
        {
            var file = Path.Combine("data", "avatars", "default.jpg");
            var ext = "jpeg";
            
            foreach (var suffix in AvailableSuffixes)
            {
                var path = Path.Combine("data", "avatars", $"{id}.{suffix}");
                if (System.IO.File.Exists(path))
                {
                    file = path;
                    ext = suffix == "jpg" ? "jpeg" : suffix;
                    break;
                }
            }

            var data = await System.IO.File.ReadAllBytesAsync(file);

            return File(data, $"image/{ext}");
        }
    }
}