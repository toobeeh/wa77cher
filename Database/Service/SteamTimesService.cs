using wa77cher.Database.Model;
using wa77cher.Util;

namespace wa77cher.Database.Service
{
    internal class SteamTimesService
    {
        private readonly AppDatabaseContext database;
        public SteamTimesService(AppDatabaseContext db) { database = db; }

        public string GetSteamTimeList()
        {
            var response = string.Join("\n", database.SteamTimes
                    .ToList()
                    .ConvertAll(item =>
                    $"`{item.TimeSpent}h` - {DateUtil.DateTimeToDiscordTimestamp(item.Timestamp, "dt")}"));
            if (response.Length == 0) response = "No records found.";
            return response;
        }

        public void ClearSteamTimes()
        {
            database.SteamTimes.RemoveRange(database.SteamTimes);
            database.SaveChanges();
        }

        public async Task LogTimeSpent(double? hours)
        {
            var entity = new SteamWeeklyTimeSpent() { TimeSpent = hours ?? 0 };
            database.SteamTimes.Add(entity);
            await database.SaveChangesAsync();
        }
    }
}
