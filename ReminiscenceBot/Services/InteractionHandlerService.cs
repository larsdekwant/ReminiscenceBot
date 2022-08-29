using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using Discord;

using ReminiscenceBot.Models;
using ReminiscenceBot.Modules;

namespace ReminiscenceBot.Services
{
    public class InteractionHandlerService
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;
        private readonly DatabaseService _database;

        public InteractionHandlerService(DiscordSocketClient client, InteractionService commands, IServiceProvider services, DatabaseService database)
        {
            _client = client;
            _commands = commands;
            _services = services;
            _database = database;
        }

        /// <summary>
        /// Initializes the interaction handler by adding all modules to the interaction service,
        /// registering the commands to the client and setting up an interaction handler.
        /// </summary>
        /// <returns>An awaitable task</returns>
        public async Task InitializeAsync()
        {
            _commands.AddTypeConverter<RorUser>(new RorUserConverter(_database));
            _commands.AddTypeConverter<List<RorUser>>(new RorUserListConverter(_database));

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
                var result = await _commands.ExecuteCommandAsync(context, _services);

                if (!result.IsSuccess)
                {
                    string msg = result.Error switch
                    {
                        InteractionCommandError.UnknownCommand => "**Unknown command**",
                        InteractionCommandError.UnmetPrecondition => "**Unmet Precondition**",
                        InteractionCommandError.BadArgs => "**Invalid number or arguments**",
                        InteractionCommandError.ConvertFailed => "**Parameter conversion failed**",
                        InteractionCommandError.Exception => "**Command exception**",
                        InteractionCommandError.Unsuccessful => "**Command could not be executed**",
                        _ => $"**Unhandled error {result.Error}**",
                    };
                    await interaction.RespondAsync($"{msg}\n{result.ErrorReason}");
                }
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
