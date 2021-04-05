using System;
using System.Net;
using System.Threading.Tasks;
using Chikatto.Cheesegull.Models;
using Chikatto.Objects;
using Newtonsoft.Json;

namespace Chikatto.Cheesegull
{
    public static class CheesegullHelper
    {
        public static async Task<BeatmapSet[]> Search(int offset, string query = "", int mode = -1, int amount = 100)
        {
            var data = $"?amount={amount}&offset={offset}";

            if (query != "")
                data += $"&query={query}";
                
            if (mode != -1)
                data += $"&mode={mode}";

            
            using var wc = new WebClient();

            try
            {
                var res = await wc.DownloadStringTaskAsync(Global.Config.DirectSearchMirror + data);
                return JsonConvert.DeserializeObject<BeatmapSet[]>(res);
            }
            catch
            {
                return Array.Empty<BeatmapSet>();
            }
        } 
    }
}