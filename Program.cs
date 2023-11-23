using System;
using wa77cher.Database;
using wa77cher.Discord;
using wa77cher.Quartz;

namespace wa77cher
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("wa77cher is never tired.");

            AppDatabaseContext.EnsureDatabaseExists();

            await DailyProfileScraper.Schedule("berwir77");

            var discordBotToken = Environment.GetEnvironmentVariable("DC_BOT_TOKEN");
            var bot = new DiscordBot(discordBotToken);
            await bot.Connect();

            bot.AddPresenceUpdatedHandler(args => PresenceUpdateHandler.HandlePresenceUpdate(args, 260826282462412801));

            await Task.Delay(-1);
        }
    }
}