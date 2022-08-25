using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using Discord;

namespace ReminiscenceBot.Services
{
    public class InteractionHandlerService
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;

        public InteractionHandlerService(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;
        }

        /// <summary>
        /// Initializes the interaction handler by adding all modules to the interaction service,
        /// registering the commands to the client and setting up an interaction handler.
        /// </summary>
        /// <returns>An awaitable task</returns>
        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.Ready += RegisterCommands;
            _client.InteractionCreated += HandleInteraction;
        }

        /// <summary>
        /// Handler for an interaction, which executes the corresponding command from the given context.
        /// </summary>
        /// <param name="interaction">The interaction to handle</param>
        /// <returns>An awaitable task</returns>
        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
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

        /// <summary>
        /// Registers the commands that have been added to the <see cref="InteractionService"/>.
        /// Commands can be registered:
        /// * To a specific server (guild), the commands are instantly available.
        /// * Globally, this takes about 1-2 hours before the commands are available on Discord.
        /// </summary>
        /// <returns>An awaitable task</returns>
        private async Task RegisterCommands()
        {
            const ulong testGuild = 652162806535421972;
            await _commands.RegisterCommandsToGuildAsync(testGuild);
        }
    }
}
