using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using wa77cher.Database;
using wa77cher.Database.Service;
using wa77cher.Discord;
using wa77cher.Quartz;

namespace wa77cher
{
    internal class Program
    {
        static readonly LogLevel LogLevel = LogLevel.Information;
        static async Task Main(string[] args)
        {
            Console.WriteLine("wa77cher is never tired.\nHere we go!\n");

            // setup database and dependency injection
            AppDatabaseContext.EnsureDatabaseExists();
            var serviceProvider = GetServiceProvider();

            // retrieve bot token
            var discordBotToken = Environment.GetEnvironmentVariable("DC_BOT_TOKEN");
            if (discordBotToken is null) throw new Exception("No discord bot token found in env");

            // init bot with dependencies for injection
            var bot = new DiscordBot(discordBotToken, serviceProvider);
            await bot.Connect();

            // add listener for discord user presence updates
            var presenceHandler = serviceProvider.GetRequiredService<PresenceUpdateHandler>();
            bot.AddPresenceUpdatedHandler(args => presenceHandler.HandlePresenceUpdate(args, 260826282462412801));

            // start scheduled jobs via DI
            var schedulerFactory = serviceProvider.GetRequiredService<ISchedulerFactory>();
            var scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();

            await Task.Delay(-1);
        }

        static ServiceProvider GetServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.SetMinimumLevel(LogLevel).AddConsole())
                .AddDbContext<AppDatabaseContext>()
                .AddSingleton<DiscordLogService>()
                .AddSingleton<SteamTimesService>()
                .AddSingleton<PresenceUpdateHandler>()
                .AddQuartz(quartz => DailyProfileScraper.Configure(quartz, "berwir77"))
                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}