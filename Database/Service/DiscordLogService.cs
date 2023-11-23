using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wa77cher.Database.Model;
using wa77cher.Util;

namespace wa77cher.Database.Service
{
    internal class DiscordLogService
    {
        private readonly AppDatabaseContext database;
        public DiscordLogService(AppDatabaseContext db) { database = db; }

        public string GetDiscordLogList()
        {
            var response = string.Join("\n", database.DiscordLog
                   .ToList()
                   .ConvertAll(item =>
                   $"`{item.EventType}` >> `{item.ActivityName}` {DateUtil.DateTimeToDiscordTimestamp(item.Timestamp, "dt")}`"));
            if (response.Length == 0) response = "No records found.";
            return response;
        }

        public void ClearDiscordLog()
        {
            database.DiscordLog.RemoveRange(database.DiscordLog);
            database.SaveChanges();
        }

        public async Task LogPresencEvents(List<DiscordPresenceEvent> events)
        {
            database.DiscordLog.AddRange(events);
            await database.SaveChangesAsync();
        }
    }
}
