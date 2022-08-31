﻿using Discord;
using Discord.Interactions;

using ReminiscenceBot.Services;
using ReminiscenceBot.Models;
using Discord.WebSocket;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace ReminiscenceBot.Modules.Commands
{
    [Group("explore", "Commands related to exploration")]
    public class ExplorationCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DatabaseService _dbService;

        private static readonly string[] _fails = { 
            "The explorers have returned with little to show for.", 
            "The Expedition came back empty handed.",
            "No new land was discovered."};

        private static readonly string[] _successFormats = {
            "The islands of **{0}** have been discovered.",
            "The continent of **{0}** has been discovered."};

        public ExplorationCommands(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        [SlashCommand("start", "Starts an expedition")]
        public async Task StartExpedition(
            [Summary(description: "Specify your crewmates by with a list of mentions (@user)")] List<RorUser> users)
        {
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
            if (rnd.NextDouble() < chance / 100)
            {
                var continents = _dbService.LoadAllDocuments<Continent>("continents");

                await ReplyAsync(string.Format(
                        _successFormats[rnd.Next(_successFormats.Length)], 
                        continents[rnd.Next(continents.Count)].Name));
            }
            else
            {
                await ReplyAsync(_fails[rnd.Next(_fails.Length)]);
            }
        }
    }
}
