using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    $"`{item.TimeSpent}h` - `{item.Timestamp.ToShortDateString()}  {item.Timestamp.ToShortTimeString()}`"));
            if (response.Length == 0) response = "No records found.";
            return response;
        }

        public void ClearSteamTimes()
        {
            database.SteamTimes.RemoveRange(database.SteamTimes);
            database.SaveChanges();
        }
    }
}
