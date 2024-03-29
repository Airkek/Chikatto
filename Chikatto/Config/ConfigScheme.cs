﻿using Newtonsoft.Json;

namespace Chikatto.Config
{
    public class ConfigScheme
    {
        [JsonProperty("database")] public SqlData Database = new();
        [JsonProperty("redis")] public RedisData Redis = new();
        
        [JsonProperty("domain")] public string Domain = "keijia.cyou";

        [JsonProperty("osu_api_token")] public string OsuApiToken = "changeme";
        
        [JsonProperty("allow_registrations")] public bool AllowRegistrations = true;
        [JsonProperty("commands_prefix")] public string CommandPrefix = "!";
        
        [JsonProperty("bot_id")] public int BotId = 999;

        [JsonProperty("enable_relax")] public bool EnableRelax = false;

        [JsonProperty("cheesegull")] public DirectData Cheesegull = new();

        [JsonProperty("misc")] public MiscData Misc = new();

        [JsonIgnore] public string SeasonalBgsJson;
    }
}