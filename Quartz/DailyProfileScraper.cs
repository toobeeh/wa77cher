using Quartz;
using Quartz.Impl;
using wa77cher.Database;
using wa77cher.Scraper;
using wa77cher.Database.Model;

namespace wa77cher.Quartz
{
    internal static class DailyProfileScraper
    {
        public static async Task Schedule(string userId)
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler scheduler = await schedFact.GetScheduler();

            var job = JobBuilder.Create<DailyProfileScraperJob>()
                .WithIdentity($"Daily profile scraper {userId}")
                .UsingJobData("userId", userId)
                .Build();

            var trigger = TriggerBuilder.Create()
                .StartNow()
                .WithDailyTimeIntervalSchedule(x => x
                .OnEveryDay()
                .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(23, 59)))
                .Build();

            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
        }
    }

    class DailyProfileScraperJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var userId = context.JobDetail.JobDataMap.GetString("userId");
            if(userId is null) throw new ArgumentNullException("userId");

            var scraper = new SteamHoursScraper();
            var hours = await scraper.ScrapeProfile(userId);

            var entity = new SteamWeeklyTimeSpent() { TimeSpent = hours ?? 0 };
            var ctx = new AppDatabaseContext();
            ctx.SteamTimes.Add(entity);
            await ctx.SaveChangesAsync();
            await ctx.DisposeAsync();
        }
    }
}
