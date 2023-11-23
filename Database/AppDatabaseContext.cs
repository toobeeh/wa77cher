using Microsoft.EntityFrameworkCore;
using SQLitePCL;
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
        private static readonly string PATH = "./data";
        private string DbPath { get { return Path.Combine(PATH, "app.db"); } }
        public static void EnsureDatabaseExists()
        {
            Directory.CreateDirectory(PATH);
            var ctx = new AppDatabaseContext();
            ctx.Database.EnsureCreated();
            ctx.Dispose();
        }

        public DbSet<SteamWeeklyTimeSpent> SteamTimes { get; set; }
        public DbSet<DiscordPresenceEvent> DiscordLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use SQLite as the database provider
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
    }
}
