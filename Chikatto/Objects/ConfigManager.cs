using System.IO;
using Chikatto.Config;
using Newtonsoft.Json;

namespace Chikatto.Objects
{
    public static class ConfigManager
    {
        private const string FileName = "config.json";

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

            cfg.SeasonalBgsJson = JsonConvert.SerializeObject(cfg.Misc.SeasonalBgs);

            return cfg;
        }
    }
}