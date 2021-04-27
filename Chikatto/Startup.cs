using System.IO;
using System.Threading;
using Chikatto.ChatCommands;
using Chikatto.Constants;
using Chikatto.Database;
using Chikatto.Database.Models;
using Chikatto.Events;
using Chikatto.Objects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chikatto
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            Db.Init();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            CommandHandler.Init();
            BanchoEventHandler.Init();

            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            if (!Directory.Exists(Path.Combine("data", "avatars")))
                Directory.CreateDirectory(Path.Combine("data", "avatars"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Global.Bot = Db.FetchOne<User>("SELECT * FROM users WHERE id = @id", new{ id = Global.Config.BotId }).GetAwaiter().GetResult();
            
            var stats = Db.FetchOne<Stats>("SELECT * FROM users_stats WHERE id = @id", new{ id = Global.Config.BotId }).GetAwaiter().GetResult();
            Global.BotCountry = Misc.CountryCodes.ContainsKey(stats.Country.ToUpper())
                ? Misc.CountryCodes[stats.Country.ToUpper()]
                : (byte) 245; // satellite provider

            var channels = Db.FetchAll<DbChannel>("SELECT * FROM bancho_channels").GetAwaiter().GetResult();
            
            foreach (var dbChannel in channels)
                Global.Channels.TryAdd(dbChannel.Name, new Channel(dbChannel));

            var maps = Db.FetchAll<Beatmap>("SELECT * FROM beatmaps").GetAwaiter().GetResult();

            Global.BeatmapManager.Cache(maps);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            new Thread(BackgroundLoops.Cleaner).Start();
        }
    }
}
