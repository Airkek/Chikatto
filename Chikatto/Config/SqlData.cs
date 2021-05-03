using Newtonsoft.Json;

namespace Chikatto.Config
{
    public class SqlData
    {
        [JsonProperty("host")] public string Host = "localhost";
        [JsonProperty("database")] public string Database = "chikatto";
        [JsonProperty("user")] public string User = "keijia";
        [JsonProperty("password")] public string Password = "changemelol";
    }
}