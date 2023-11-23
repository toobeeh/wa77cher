using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wa77cher.Database;
using wa77cher.Database.Model;
using wa77cher.Scraper;

namespace wa77cher.Discord
{
    internal class DiscordCommands : BaseCommandModule
    {
        public AppDatabaseContext Database { private get; set; }

        [Command("list")]
        [Description("List recorded data")]
        [RequireRoles(RoleCheckMode.SpecifiedOnly, 893786618446643200)]
        public async Task List(CommandContext context, string source = "steam")
        {
            if(source != "steam" && source != "discord")
            {
                await context.RespondAsync("Invalid source " + source);
                return;
            }

            var response = "";

            if (source == "steam")
            {
                response = String.Join("\n", Database.SteamTimes
                    .ToList()
                    .ConvertAll(item =>
                    $"`{item.TimeSpent}h` - `{item.Timestamp.ToShortDateString()}  {item.Timestamp.ToShortTimeString()}`"));
            }
            else if(source == "discord")
            {
                response = String.Join("\n", Database.DiscordLog
                   .ToList()
                   .ConvertAll(item =>
                   $"`{item.EventType}` >> `{item.ActivityName}` `{item.Timestamp.ToShortDateString()}  {item.Timestamp.ToShortTimeString()}`"));
            }

            if (response.Length == 0) response = "No records found.";
            await context.RespondAsync(response);
        }


        [Command("clear")]
        [Description("Clear recorded data")]
        [RequireRoles(RoleCheckMode.SpecifiedOnly, 893786618446643200)]
        public async Task Clear(CommandContext context, string source = "steam")
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
                if(source == "steam") Database.SteamTimes.RemoveRange(Database.SteamTimes);
                else if (source == "discord") Database.DiscordLog.RemoveRange(Database.DiscordLog);

                Database.SaveChanges();
                await interaction.Result.Interaction.CreateResponseAsync(
                   DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                   new DiscordInteractionResponseBuilder().WithContent("Cleared records."));
            }

            message.ClearComponents();
            await response.ModifyAsync(message);
        }
    }
}
