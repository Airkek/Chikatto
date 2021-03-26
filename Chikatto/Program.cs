using Chikatto.Database;
using Chikatto.Objects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Chikatto
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Global.Config = ConfigManager.Read();
            Global.Database = GulagDbContext.Create();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}