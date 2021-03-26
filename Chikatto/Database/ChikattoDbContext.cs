using Microsoft.EntityFrameworkCore;
using Chikatto.Database.Models;
using Chikatto.Objects;
using User = Chikatto.Database.Models.User;

namespace Chikatto.Database
{
    public class GulagDbContext : DbContext
    {
        public GulagDbContext(DbContextOptions<GulagDbContext> options) : base(options)
        {
        }
        
        public static GulagDbContext Create()
        {
            var builder = new DbContextOptionsBuilder<GulagDbContext>().UseMySql(Global.DbConnectionString);
            return new GulagDbContext(builder.Options);
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Stats> Stats { get; set; }
        public DbSet<VanillaScore> VanillaScores { get; set; }
        public DbSet<RelaxScore> RelaxScores { get; set; }
        public DbSet<PilotScore> PilotScores { get; set; }
        public DbSet<Beatmap> Beatmaps { get; set; }
    }
}