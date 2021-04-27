using Newtonsoft.Json;

namespace Chikatto.Objects
{
    public class ConfigScheme
    {
        [JsonProperty("db_host")] public string DatabaseHost = "localhost";
        [JsonProperty("db_name")] public string DatabaseName = "chikatto";
        [JsonProperty("db_user")] public string DatabaseUser = "keijia";
        [JsonProperty("db_password")] public string DatabasePassword = "changemelol";

        [JsonProperty("osu_api_token")] public string OsuApiToken = "changeme";
        
        [JsonProperty("bot_id")] public int BotId = 999;
        [JsonProperty("commands_prefix")] public string CommandPrefix = "!";

        [JsonProperty("allow_registrations")] public bool AllowRegistrations = true;
        
        [JsonProperty("osu_direct_search_mirror")] public string DirectCheesegullMirror = "https://api.chimu.moe/cheesegull/";
        [JsonProperty("osu_direct_download_mirror")] public string DirectDownloadMirror = "https://chimu.moe/d/";

        [JsonProperty("osu_logo_ingame")] public string LogoIngame = "https://osu.shizofrenia.pw/static/images/logo_ingame.png";
        [JsonProperty("osu_logo_click_url")] public string LogoClickUrl = "https://github.com/Airkek/Chikatto";

        [JsonProperty("osu_seasonal_bgs")] public string[] SeasonalBgs = { "https://akatsuki.pw/static/flower.png" };

        [JsonIgnore] public string SeasonalBgsJson;
    }
}