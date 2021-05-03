using Newtonsoft.Json;

namespace Chikatto.Config
{
    public class RedisData
    {
        [JsonProperty("using_redis")] public bool Enabled = true;
        [JsonProperty("host")] public string Host = "localhost";
        [JsonProperty("port")] public uint Port = 6379;
        [JsonProperty("password")] public string Password = "";
    }
}