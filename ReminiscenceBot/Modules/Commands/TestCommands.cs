using System.Reflection;

using Discord;
using Discord.Interactions;

using ReminiscenceBot.Services;
using ReminiscenceBot.Models;
using Discord.WebSocket;
using Discord.Commands;

namespace ReminiscenceBot.Modules.Commands
{
    /// <summary>
    /// This is just a class to test some of Discord.NET's functionalities, will be removed later.
    /// </summary>
    public class TestCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly InteractionService _commands;

        public TestCommands(InteractionService commands)
        {
            _commands = commands;
        }

        [SlashCommand("ping", "Send the bot a ping!")]
        public async Task Ping()
        {
            await RespondAsync($"Server responded in {Context.Client.Latency}ms");
        }

        [SlashCommand("menu-test", "Test menu functionality")]
        public async Task TestMenu()
        {
            var menu = new SelectMenuBuilder()
                .WithCustomId("test")
                .WithPlaceholder("Select an item")
                .WithMaxValues(2)
                .AddOption(Context.User.Username, "opt-a", "This is option A")
                .AddOption("Option B", "opt-b", "This is option B")
                .AddOption("Option C", "opt-c", "This is option C");

            await RespondAsync("Choose an option!", components: new ComponentBuilder().WithSelectMenu(menu).Build(), ephemeral: true);
        }

        [ComponentInteraction("test", true)]
        public async Task TestMenuHandler(string[] selections)
        {
            await RespondAsync($"You selected {selections.Length} options");
        }

        [SlashCommand("echo", "Echo an input by pressing a button")]
        public async Task EchoSubcommand(string input)
            => await RespondAsync(components: new ComponentBuilder().WithButton("Echo", $"echoButton_{input}").Build());

        [ComponentInteraction("echoButton_*", true)]
        public async Task EchoButton(string input)
            => await RespondAsync(input);
    }
}
