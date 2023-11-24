using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wa77cher.Database;
using wa77cher.Database.Model;
using wa77cher.Database.Service;
using wa77cher.Scraper;
using wa77cher.Util;

namespace wa77cher.Discord
{
    internal class DiscordCommands : BaseCommandModule
    {
        private readonly SteamTimesService steamService;
        private readonly DiscordLogService logService;
        public DiscordCommands(SteamTimesService ss, DiscordLogService ls) { steamService = ss; logService = ls; }

        [Command("list")]
        [Description("List recorded data")]
        [RequireRoles(RoleCheckMode.MatchIds, 893786618446643200)]
        public async Task List(CommandContext context, [Description("The statistic to view: either 'steam' (default) or 'discord'.")] string source = "steam")
        {
            var response = "Invalid source " + source;
            if (source == "steam") response = steamService.GetSteamTimeList();
            else if(source == "discord")  response = logService.GetDiscordLogList();

            var pages = context.Client.GetInteractivity().GeneratePagesInContent(response, DSharpPlus.Interactivity.Enums.SplitType.Line);
            await context.Client.GetInteractivity().SendPaginatedMessageAsync(context.Channel, context.User, pages);
        }

        [Command("timeline")]
        [Description("View a daily timeline of discord events")]
        [RequireRoles(RoleCheckMode.MatchIds, 893786618446643200)]
        public async Task Timeline(CommandContext context)
        {
            var days = logService.GetDiscordDailySummaries();
            var pages = days.Keys.ToList().ConvertAll(key =>
            {
                var summary = days[key];
                var page = new Page();
                page.Content = $"## {DateUtil.DateTimeToDiscordTimestamp(key, "D")}\n {summary}";
                return page;
            });

            await context.Client.GetInteractivity().SendPaginatedMessageAsync(context.Channel, context.User, pages);
        }

        [Command("clear")]
        [Description("Clear recorded data")]
        [RequireRoles(RoleCheckMode.MatchIds, 893786618446643200)]
        public async Task Clear(CommandContext context, [Description("The statistic to clear: either 'steam' (default) or 'discord'.")] string source = "steam")
        {
            if (source != "steam" && source != "discord")
            {
                await context.RespondAsync("Invalid source " + source);
                return;
            }

            var message = new DiscordMessageBuilder()
                .WithContent("Warning: Clearing records is irreversible.")
                .AddComponents(
                new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, "clear", "Clear records"));

            var response = await context.RespondAsync(message);

            var interaction = await response.WaitForButtonAsync(context.User, TimeSpan.FromMinutes(1));
            if (!interaction.TimedOut)
            {
                if (source == "steam") steamService.ClearSteamTimes();
                else if (source == "discord") logService.ClearDiscordLog();

                await interaction.Result.Interaction.CreateResponseAsync(
                   DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                   new DiscordInteractionResponseBuilder().WithContent("Cleared records."));
            }

            message.ClearComponents();
            await response.ModifyAsync(message);
        }

        [Command("steamhrs")]
        [Description("View spent hours of a user's steam profile")]
        [RequireRoles(RoleCheckMode.MatchIds, 893786618446643200)]
        public async Task SteamHrs(CommandContext context, [Description("The steam user id of the user to fetch (default: 'berwir77')")] string username = "berwir77")
        {
            double? hours = null;
            double? bwHours = null;
            try
            {
                var scraper = new SteamHoursScraper();
                hours = await scraper.ScrapeProfile(username);
                if(username != "berwir77") bwHours = await scraper.ScrapeProfile("berwir77");
            }
            catch { }

            if(hours is null)
            {
                await context.RespondAsync("Something went wrong. Maybe this user doesn't exist.");
            }
            else if(hours is not null && bwHours is null)
            {
                await context.RespondAsync($"User {username} has spent {hours}h in the last two weeks playing steam games.");
            }
            else if(hours is not null && bwHours is not null)
            {
                await context.RespondAsync($"User {username} has spent {hours}h in the last two weeks playing steam games.\nThat's {Math.Abs((double)bwHours-(double)hours)}h {(bwHours > hours ? "less" : "more")} than berwir77!");
            }
        }
    }
}
