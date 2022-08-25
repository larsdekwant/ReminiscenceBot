using Discord;
using Discord.Interactions;

using ReminiscenceBot.Services;
using ReminiscenceBot.Models;
using Discord.WebSocket;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace ReminiscenceBot.Modules
{
    [Discord.Interactions.Group("explore", "Commands related to exploration")]
    public class ExplorationCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DatabaseService _dbService;

        public ExplorationCommands(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        [SlashCommand("start", "Starts an expedition")]
        public async Task TestExpedition(
            [Summary(description: "Specify your crewmates by with a list of mentions (@user)")] string listOfCrew)
        {
            // Match all mention strings of the form <@12345> (normal) and <@!12345> (nickname)
            var crewIds = Regex
                .Matches(listOfCrew, @"<@!?(?<id>\d+)>")
                .Select(m => ulong.Parse(m.Groups["id"].Value));

            List<RorUser> users = _dbService.LoadDocuments("users", Builders<RorUser>.Filter.In(x => x.Discord.Id, crewIds));

            // TODO: mention users that do not have an account and can thus not join.

            await RespondAsync("You are going on a journey with: " + string.Join(" ", users.Select(u => u.Discord.Mention)));
        }       
    }
}
