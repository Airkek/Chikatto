using System;
using System.Net;
using System.Threading.Tasks;

namespace Chikatto.Utils
{
    public static class IpApi
    {
        public static async Task<string> FetchLocation(string ip)
        {
            using var wc = new WebClient();

            var country = "xx";

            try
            {
                var response = await wc.DownloadStringTaskAsync($"http://ip-api.com/line/{ip}");
                var split = response.Split(new[] {"\n", "\r\n"}, StringSplitOptions.None);

                if (split[0] == "success")
                {
                    country = split[2].ToLower();
                }
                else
                    XConsole.Log($"Failed to fetch location for {ip} ({split[0]}: {split[1]})", back: ConsoleColor.Yellow);
            }
            catch
            {
                XConsole.Log($"Failed to fetch location for {ip} (Unknown error)", back: ConsoleColor.Yellow);
            }

            return country;
        }
    }
}