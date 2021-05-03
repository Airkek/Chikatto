using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Chikatto.Enums;
using Chikatto.Utils.Cheesegull.Models;

namespace Chikatto.Utils.Cheesegull
{
    public static class CheesegullHelper
    {
        public static async Task<BeatmapSet[]> Search(int offset, string query = "", int mode = -1, DirectRankedStatus status = DirectRankedStatus.All, int amount = 100)
        {
            var data = $"?amount={amount}&offset={offset}";

            if (query != "")
                data += $"&query={query}";
                
            if (mode != -1)
                data += $"&mode={mode}";

            if (status != DirectRankedStatus.All)
                data += $"&status={RankedStatusHelper.ConvertToCheesegull(status)}";

            using var wc = new WebClient();

            try
            {
                var res = await wc.DownloadStringTaskAsync(Global.Config.Cheesegull.ApiMirror + "search" + data);
                return JsonConvert.DeserializeObject<BeatmapSet[]>(res) ?? Array.Empty<BeatmapSet>();
            }
            catch
            {
                return Array.Empty<BeatmapSet>();
            }
        }

        public static async Task<BeatmapSet> GetSet(int id)
        {
            using var wc = new WebClient();
            
            try
            {
                var res = await wc.DownloadStringTaskAsync(Global.Config.Cheesegull.ApiMirror + "s/" + id);
                return JsonConvert.DeserializeObject<BeatmapSet>(res);
            }
            catch
            {
                return null;
            }
        }    
        
        public static async Task<Beatmap> GetMap(int id)
        {
            using var wc = new WebClient();
            
            try
            {
                var res = await wc.DownloadStringTaskAsync(Global.Config.Cheesegull.ApiMirror + "b/" + id);
                return JsonConvert.DeserializeObject<Beatmap>(res);
            }
            catch
            {
                return null;
            }
        }    
    }
}