using Quartz;
using Quartz.Impl;
using wa77cher.Database;
using wa77cher.Scraper;
using wa77cher.Database.Model;
using wa77cher.Database.Service;
using Microsoft.Extensions.Logging;

namespace wa77cher.Quartz
{
    internal static class DailyProfileScraper
    {
        public static void Configure(IServiceCollectionQuartzConfigurator configurator, string userId)
        {
            var jobId = new JobKey($"Daily profile scraper {userId}");

            configurator.AddJob<DailyProfileScraperJob>(job => job
                .WithIdentity(jobId)
                .UsingJobData("userId", userId));

            configurator.AddTrigger(trigger => trigger
                .StartNow()
                .WithDailyTimeIntervalSchedule(x => x
                .OnEveryDay()
                .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(23, 59))));

            return;
        }
    }

    class DailyProfileScraperJob : IJob
    {
        private readonly SteamTimesService steamTimesService;
        private readonly ILogger<DailyProfileScraperJob> logger;
        public DailyProfileScraperJob(SteamTimesService service, ILogger<DailyProfileScraperJob> logger) 
        { 
            steamTimesService = service; 
            this.logger = logger; 
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var userId = context.JobDetail.JobDataMap.GetString("userId");
            if(userId is null) throw new Exception("userId was null");

            var scraper = new SteamHoursScraper();
            var hours = await scraper.ScrapeProfile(userId);

            await steamTimesService.LogTimeSpent(hours);

            logger.LogInformation("Scraped steam hours for " + userId);
        }
    }
}
