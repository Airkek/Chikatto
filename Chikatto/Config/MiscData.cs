using Newtonsoft.Json;

namespace Chikatto.Config
{
    public class MiscData
    {
        [JsonProperty("logo_ingame")] public string LogoIngame = "https://osu.shizofrenia.pw/static/images/logo_ingame.png";
        [JsonProperty("logo_click_url")] public string LogoClickUrl = "https://github.com/Airkek/Chikatto";

        [JsonProperty("seasonal_bgs")] public string[] SeasonalBgs = { "https://akatsuki.pw/static/flower.png" };
    }
}