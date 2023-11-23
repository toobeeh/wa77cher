using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wa77cher.Database;
using wa77cher.Database.Model;
using wa77cher.Database.Service;
using wa77cher.Quartz;

namespace wa77cher.Discord
{
    internal class PresenceUpdateHandler
    {
        private readonly DiscordLogService service;
        private readonly ILogger<DailyProfileScraperJob> logger;
        public PresenceUpdateHandler(DiscordLogService service, ILogger<DailyProfileScraperJob> logger) { this.service = service; this.logger = logger; }

        public async Task HandlePresenceUpdate(PresenceUpdateEventArgs args, ulong userIdFilter)
        {
            if(args.User.Id == userIdFilter)
            {
                List<DiscordPresenceEvent> events = new();

                var before = args.PresenceBefore.Activity;
                var after = args.PresenceAfter.Activity;

                // filter out non-games
                if (before is not null && before.ActivityType != DSharpPlus.Entities.ActivityType.Playing) before = null;
                if (after is not null && after.ActivityType != DSharpPlus.Entities.ActivityType.Playing) after = null;

                // if activity ended
                if (before is null && after is not null)
                {
                   events.Add(new DiscordPresenceEvent { 
                       ActivityName = after.Name ?? "Unknown",
                       EventType = PresenceEventType.Start
                   });
                }

                // if activity started
                else if (before is not null && after is null)
                {
                    events.Add(new DiscordPresenceEvent
                    {
                        ActivityName = before.Name ?? "Unknown",
                        EventType = PresenceEventType.End
                    });
                }

                // if different activity
                else if(before is not null && after is not null && after.Name != before.Name)
                {
                    events.Add(new DiscordPresenceEvent
                    {
                        ActivityName = before.Name ?? "Unknown",
                        EventType = PresenceEventType.End
                    });
                    events.Add(new DiscordPresenceEvent
                    {
                        ActivityName = after.Name ?? "Unknown",
                        EventType = PresenceEventType.Start
                    });
                }

                await service.LogPresencEvents(events);

                logger.LogInformation("Processed presence events for " + userIdFilter);
            }
        }
    }
}
