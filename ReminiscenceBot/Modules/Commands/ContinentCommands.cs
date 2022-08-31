using Discord;
using Discord.Interactions;

using ReminiscenceBot.Services;
using ReminiscenceBot.Models;
using MongoDB.Driver;

namespace ReminiscenceBot.Modules.Commands
{
    [Group("continents", "Commands related to continents")]
    public class ContinentCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DatabaseService _dbService;

        public ContinentCommands(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        [SlashCommand("add", "Adds a continent to the catalog of explorable continents")]
        public async Task AddContinent([ComplexParameter] Continent continent)
        {
            _dbService.UpsertDocument("continents",
                Builders<Continent>.Filter.Eq(c => c.Name, continent.Name),
                continent);

            await RespondAsync($"Added continent `{continent.Name}` to the catalog!");
        }

        [SlashCommand("list", "Lists all explorable continents")]
        public async Task ListAllContinents()
        {
            var continents = _dbService.LoadAllDocuments<Continent>("continents");

            var embedBuilder = new EmbedBuilder()
                .WithTitle("List of all available continents")
                .WithDescription(string.Join('\n', continents.Select(c => $"**{c.Name}**\n{c.Description}\n")))
                .WithCurrentTimestamp();

            await RespondAsync(embed: embedBuilder.Build());
        }
    }
}
