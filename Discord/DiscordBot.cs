using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using wa77cher.Database;
using wa77cher.Database.Service;

namespace wa77cher.Discord
{
    internal class DiscordBot
    {
        private DiscordClient Client;

        public DiscordBot(string token, ServiceProvider serviceProvider)
        {
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });

            var commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { "bw" },
                CaseSensitive = false,
                Services = serviceProvider
            });
            commands.RegisterCommands<DiscordCommands>();

            Client.UseInteractivity();
        }

        public async Task Connect()
        {
            await Client.ConnectAsync();
        }

        public void AddPresenceUpdatedHandler(Func<PresenceUpdateEventArgs, Task> handler)
        {
            async Task presenceHandler(DiscordClient client, PresenceUpdateEventArgs args) {
                await handler(args);
            }

            Client.PresenceUpdated += presenceHandler;
        }
    }
}
