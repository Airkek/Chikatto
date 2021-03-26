using System.Linq;
using Chikatto.Database;
using Chikatto.Objects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var users = context.Users.AsNoTracking().AsEnumerable();
            foreach (var user in users)
            {
                if (user.Id == 1){
                    
                    Global.Bot = user;
                    continue;
                }

                Global.IdCache[user.SafeName] = user.Id;
            }   
        }
    }
}
