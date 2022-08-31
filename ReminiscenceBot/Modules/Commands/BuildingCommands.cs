using Discord;
using Discord.Interactions;

using ReminiscenceBot.Services;
using ReminiscenceBot.Models;
using MongoDB.Driver;

namespace ReminiscenceBot.Modules.Commands
{
    [Group("building", "Commands related to buildings")]
    public class BuildingCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DatabaseService _dbService;

        public BuildingCommands(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        [SlashCommand("add", "Adds a building to the catalog of available buildings")]
        public async Task AddBuilding([ComplexParameter] Building building)
        {
            _dbService.UpsertDocument("buildings",
                Builders<Building>.Filter.Eq(b => b.Name, building.Name),
                building);

            await RespondAsync($"Added building `{building.Name}` to the catalog!");
        }

        [SlashCommand("list", "Lists all available buildings")]
        public async Task ListAllBuildings()
        {
            var buildings = _dbService.LoadAllDocuments<Building>("buildings");

            var embedBuilder = new EmbedBuilder()
                .WithTitle("List of all available buildings")
                .WithDescription(string.Join('\n', buildings.Select(b => $"{b.Name} (+{b.BonusChance}%)")))
                .WithCurrentTimestamp();

            await RespondAsync(embed: embedBuilder.Build());
        }
    }
}
