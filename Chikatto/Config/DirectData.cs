using Newtonsoft.Json;

namespace Chikatto.Config
{
    public class DirectData
    {
        [JsonProperty("api_mirror")] public string ApiMirror = "https://api.chimu.moe/cheesegull/"; 
        [JsonProperty("download_mirror")] public string DownloadMirror = "https://chimu.moe/d/"; 
    }
}