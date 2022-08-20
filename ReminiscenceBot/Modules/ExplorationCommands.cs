using Discord;
using Discord.Interactions;

using ReminiscenceBot.Services;
using ReminiscenceBot.Models;

namespace ReminiscenceBot.Modules
{
    [Group("exploration", "Commands related to exploration")]
    public class ExplorationCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DatabaseService _dbService;

        public ExplorationCommands(DatabaseService dbService)
        {
            _dbService = dbService;
        }
       
        [SlashCommand("ping", "Send the bot a ping!")]
        public async Task Ping()
        {
            await RespondAsync($"Server responded in {Context.Client.Latency}ms");
        }

        [SlashCommand("add-user", "Add a user to the database")]
        public async Task AddUser(string name)
        {
            _dbService.InsertDocument("Users", new User { DiscordId = Context.User.Id, Name = name });
            await RespondAsync($"Added new user {name} to the database!");
        }

        [SlashCommand("menu-test", "Test menu functionality")]
        public async Task TestMenu()
        {
            var menu = new SelectMenuBuilder()
                .WithCustomId("test")
                .WithPlaceholder("Select an item")
                .WithMaxValues(2)
                .AddOption("Option A", "opt-a", "This is option A")
                .AddOption("Option B", "opt-b", "This is option B")
                .AddOption("Option C", "opt-c", "This is option C");

            await RespondAsync("Choose an option!", components: new ComponentBuilder().WithSelectMenu(menu).Build());
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
