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
        public async Task StartExpedition(
            [Summary(description: "Specify your crewmates by with a list of mentions (@user)")] string listOfCrew)
        {
            // Match all mention strings of the form <@12345> (normal) and <@!12345> (nickname)
            var crewIds = Regex
                .Matches(listOfCrew, @"<@!?(?<id>\d+)>")
                .Select(m => ulong.Parse(m.Groups["id"].Value));

            List<RorUser> users = _dbService.LoadDocuments("users", Builders<RorUser>.Filter.In(x => x.Discord.Id, crewIds));

            // TODO: mention users that do not have an account and can thus not join.

            RorUser captain = _dbService.LoadDocuments("users",
                Builders<RorUser>.Filter.Eq(u => u.Discord.Id, Context.User.Id)).First();

            double chance = captain.Player.Buildings.Values.Sum();

            var embed = new EmbedBuilder()
                .WithTitle($"An epic expedition with {chance.ToString("0.#")}% chance to succeed.")
                .WithAuthor(Context.User)
                .AddField("Captain", captain.Discord.Mention)
                .AddField("Crew", string.Join(" ", users.Select(u => u.Discord.Mention)))
                .WithCurrentTimestamp();
            await RespondAsync(embed: embed.Build());

            // Perform an expedition with the success chance.
            Random rnd = new Random();
            if (rnd.NextDouble() < (chance / 100))
            {
                await ReplyAsync("The expedition was succesful! :tada:");
            } else
            {
                await ReplyAsync("The expedition failed :(");
            }                        
        }       
    }
}
