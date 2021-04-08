using System;
using System.Net;
using System.Threading.Tasks;
using Chikatto.Cheesegull.Models;
using Chikatto.Enums;
using Chikatto.Objects;
using Chikatto.Utils;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;

namespace Chikatto.Cheesegull
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
                var res = await wc.DownloadStringTaskAsync(Global.Config.DirectSearchMirror + data);
                return JsonConvert.DeserializeObject<BeatmapSet[]>(res) ?? Array.Empty<BeatmapSet>();
            }
            catch
            {
                return Array.Empty<BeatmapSet>();
            }
        } 
    }
}