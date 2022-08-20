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

        // Construct and assign the program member variables.
        private Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds,
                LogLevel = LogSeverity.Info
            });

            _commands = new InteractionService(_client, new InteractionServiceConfig
            {
                LogLevel = LogSeverity.Info,
                DefaultRunMode = RunMode.Async
            });

            // Injected into the services/models using Dependency Injection
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(new DatabaseService("test"))
                .AddSingleton<SlashCommandService>()
                .BuildServiceProvider(); ;

            // Subscribe the event handlers and loggers.
            _client.Ready += async () => { await _commands.RegisterCommandsToGuildAsync(652162806535421972); };          
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

        // Log info to the console.
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}