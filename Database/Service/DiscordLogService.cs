﻿using Microsoft.EntityFrameworkCore;
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
                   $"`{item.EventType}` >> `{item.ActivityName}` {DateUtil.DateTimeToDiscordTimestamp(item.Timestamp, "dt")}"));
            if (response.Length == 0) response = "No records found.";
            return response;
        }

        public Dictionary<DateTime, string> GetDiscordDailySummaries()
        {
            var days = database.DiscordLog
                   .GroupBy(e => e.Timestamp.Date)
                   .ToList();

            var mappedDays = new Dictionary<DateTime, string>();
            foreach(var day in days)
            {
                if (day is null) continue;
                var events = day.ToList();
                string dayList = "";

                DiscordPresenceEvent? next;
                for(int i=0; i < events.Count; i++)
                {
                    next = i < (events.Count - 1) ? events[i + 1] : null;
                    var evt = events[i];

                    if(evt.EventType == PresenceEventType.Start)
                    {
                        if(next is not null && next.EventType == PresenceEventType.End && next.ActivityName == evt.ActivityName)
                        {
                            var span = next.Timestamp.Subtract(evt.Timestamp);
                            var duration = $"{(int)span.TotalHours}h {(int)span.TotalMinutes % 60}m";
                            dayList += $"{DateUtil.DateTimeToDiscordTimestamp(evt.Timestamp, "t")} Played {evt.ActivityName} for {duration}\n";
                            i++;
                        }
                        else if(evt.EventType == PresenceEventType.Start)
                        {
                           dayList += $"{DateUtil.DateTimeToDiscordTimestamp(evt.Timestamp, "t")} Started playing {evt.ActivityName}\n";
                        }
                        else if (evt.EventType == PresenceEventType.End)
                        {
                            dayList += $"{DateUtil.DateTimeToDiscordTimestamp(evt.Timestamp, "t")} Quit {evt.ActivityName}\n";
                        }
                    }
                }

                if(events.Count() > 0) mappedDays.Add(events[0].Timestamp.Date, dayList);
            }

            return mappedDays;
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
