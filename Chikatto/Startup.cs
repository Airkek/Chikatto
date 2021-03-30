using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Chikatto.Constants;
using Chikatto.Database;
using Chikatto.Objects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chikatto
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<GulagDbContext>(optionsBuilder => optionsBuilder.UseMySql(Global.DbConnectionString));
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, GulagDbContext context)
        {
            context.Database.EnsureCreated();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Global.Bot = context.Users.Find(Global.Config.BotId);

            Global.BotCountry = Misc.CountryCodes.ContainsKey(Global.Bot.Country.ToUpper())
                ? Misc.CountryCodes[Global.Bot.Country.ToUpper()]
                : (byte) 245; // satellite provider

            var channels = context.Channels.AsNoTracking().AsEnumerable();
            
            foreach (var dbChannel in channels)
                Global.Channels.Add(dbChannel.Name, new Channel(dbChannel));
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
