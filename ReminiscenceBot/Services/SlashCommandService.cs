using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using Discord;

namespace ReminiscenceBot.Services
{
    public class SlashCommandService
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;

        public SlashCommandService(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;
        }

        // Adds all the command modules and assigns an interaction handler.
        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.InteractionCreated += HandleInteractionCommand;
        }

        // Handler for an interaction command, which executes the corresponding command from the given context.
        private async Task HandleInteractionCommand(SocketInteraction interaction)
        {
            try
            {
                // Create the execution context
                SocketInteractionContext context = new SocketInteractionContext(_client, interaction);
                await _commands.ExecuteCommandAsync(context, _services);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                // Feedback to the user of the failed command
                if (interaction.Type == InteractionType.ApplicationCommand)
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}
