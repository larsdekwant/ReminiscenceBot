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
    }
}
