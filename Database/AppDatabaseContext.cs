using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wa77cher.Database.Model;

namespace wa77cher.Database
{
    internal class AppDatabaseContext : DbContext
    {
        public static void EnsureDatabaseExists()
        {
            var ctx = new AppDatabaseContext();
            ctx.Database.EnsureCreated();
            ctx.Dispose();
        }

        public DbSet<SteamWeeklyTimeSpent> SteamTimes { get; set; }
        public DbSet<DiscordPresenceEvent> DiscordLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use SQLite as the database provider
            optionsBuilder.UseSqlite("Data Source=app.db");
        }
    }
}
