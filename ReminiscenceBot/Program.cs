using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ReminiscenceBot.Services;

namespace ReminiscenceBot
{
    public class Program
    {
        public static Task Main(string[] args) => new Program().MainAsync();

        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _config;

        // Construct and assign the program member variables.
        private Program()
        {
            _config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });

            _commands = new InteractionService(_client, new InteractionServiceConfig
            {
                LogLevel = LogSeverity.Info,
                DefaultRunMode = RunMode.Async
            });

            _services = ConfigureServices();

            // Subscribe the event handlers and loggers.
            _client.Ready += RegisterCommands;
            _client.Log += Log;
            _commands.Log += Log;
        }

        // The main bot method that runs asynchronously.
        public async Task MainAsync()
        {
            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("BOT_TOKEN"));
            await _client.StartAsync();

            await _services.GetRequiredService<SlashCommandService>().InitializeAsync();

            // Keep the bot running until it is closed.
            await Task.Delay(Timeout.Infinite);
        }

        // Configure the services with Dependency Injection.
        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton<SlashCommandService>()
                .BuildServiceProvider();
        }

        // Register the commands to a test server.
        private async Task RegisterCommands()
        {
            await _commands.RegisterCommandsToGuildAsync(652162806535421972);
        }

        // Log info to the console.
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}