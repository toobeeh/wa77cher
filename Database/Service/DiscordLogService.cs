﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                   $"`{item.EventType}` >> `{item.ActivityName}` `{item.Timestamp.ToShortDateString()}  {item.Timestamp.ToShortTimeString()}`"));
            if (response.Length == 0) response = "No records found.";
            return response;
        }

        public void ClearDiscordLog()
        {
            database.DiscordLog.RemoveRange(database.DiscordLog);
            database.SaveChanges();
        }
    }
}