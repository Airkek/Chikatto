using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Chikatto.Constants;
using Chikatto.Database;
using Chikatto.Database.Models;
using Chikatto.Objects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dapper;
using MySql.Data.MySqlClient;

namespace Chikatto
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            DatabaseHelper.Init();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            if (!Directory.Exists(Path.Combine("data", "avatars")))
                Directory.CreateDirectory(Path.Combine("data", "avatars"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Global.Bot = DatabaseHelper.FetchOne<User>("SELECT * FROM users WHERE id = @id", new{ id = Global.Config.BotId }).GetAwaiter().GetResult();

            Global.BotCountry = Misc.CountryCodes.ContainsKey(Global.Bot.Country.ToUpper())
                ? Misc.CountryCodes[Global.Bot.Country.ToUpper()]
                : (byte) 245; // satellite provider

            var channels = DatabaseHelper.FetchAll<DbChannel>("SELECT * FROM channels").GetAwaiter().GetResult();
            
            foreach (var dbChannel in channels)
                Global.Channels.TryAdd(dbChannel.Name, new Channel(dbChannel));

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
