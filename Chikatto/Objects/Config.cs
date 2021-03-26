using System.IO;
using Newtonsoft.Json;

namespace Chikatto.Objects
{
    public class ConfigScheme
    {
        [JsonProperty("db_host")] public string DatabaseHost = "localhost";
        [JsonProperty("db_name")] public string DatabaseName = "chikatto";
        [JsonProperty("db_user")] public string DatabaseUser = "keijia";
        [JsonProperty("db_password")] public string DatabasePassword = "changemelol";
        
        [JsonProperty("osu_direct_mirror")] public string DirectMirror = "chimu.moe";

        [JsonProperty("osu_logo_ingame")] public string LogoIngame = "https://osu.shizofrenia.pw/static/images/logo_ingame.png";
        [JsonProperty("osu_logo_click_url")] public string LogoClickUrl = "https://github.com/Airkek/Chikatto";
    }
    
    public class ConfigManager
    {
        public const string FileName = "config.json";

        public static ConfigScheme Read()
        {
            ConfigScheme cfg;
            
            if (!File.Exists(FileName))
            {
                cfg = new ConfigScheme();
                File.WriteAllText(FileName, JsonConvert.SerializeObject(cfg, Formatting.Indented));
            }
            else
            {
                var file = File.ReadAllText(FileName);
                cfg = JsonConvert.DeserializeObject<ConfigScheme>(file);
                File.WriteAllText(FileName, JsonConvert.SerializeObject(cfg, Formatting.Indented)); //update config file to latest scheme
            }

            return cfg;
        }
    }
}