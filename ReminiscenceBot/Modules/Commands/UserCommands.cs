using Discord;
using Discord.Interactions;

using ReminiscenceBot.Services;
using ReminiscenceBot.Models;
using Discord.WebSocket;
using MongoDB.Driver;
using System.Numerics;
using ReminiscenceBot.Modules.AutocompleteHandlers;

namespace ReminiscenceBot.Modules.Commands
{
    [Group("user", "Commands related to users")]
    public class UserCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DatabaseService _dbService;

        public UserCommands(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        [SlashCommand("profile", "Displays a profile for a given user")]
        [Help("This is some useless help message.")]
        public async Task ShowUserProfile(RorUser user)
        {
            var dcUser = Context.Guild.GetUser(user.Discord.Id);

            var embedBuilder = new EmbedBuilder()
                .WithThumbnailUrl(dcUser.GetAvatarUrl())
                .WithTitle("Realms of Reminiscence profile")
                .WithAuthor(dcUser)
                .AddField("Minecraft name", user.Player.McUsername)
                .AddField("Character name", user.Player.RorName)
                .AddField("Class", user.Player.Class)
                .AddField("Race", user.Player.Race)
                .WithColor(Color.Blue)
                .WithCurrentTimestamp();

            await RespondAsync(embed: embedBuilder.Build());
        }

        [SlashCommand("set", "Set or update your user profile information.")]
        public async Task SetUser([ComplexParameter] PlayerInfo player)
        {
            _dbService.UpsertDocument("users",
                Builders<RorUser>.Filter.Eq(u => u.Discord.Id, Context.User.Id),
                new RorUser(new DiscordInfo(Context.User), player));

            await RespondAsync($"Updated your user profile!");
        }

        [SlashCommand("list", "Lists all users of the bot")]
        public async Task ListAllUsers()
        {
            List<RorUser> rorUsers = _dbService.LoadAllDocuments<RorUser>("users");

            var embedBuilder = new EmbedBuilder()
                .WithTitle("List of all Realms of Reminiscence users")
                .WithDescription(string.Join('\n', rorUsers.Select(
                    rorUser => $"{rorUser.Discord.Mention}: {rorUser.Player.RorName} ({rorUser.Player.Class})")))
                .WithCurrentTimestamp();

            await RespondAsync(embed: embedBuilder.Build());
        }

        [SlashCommand("add-building", "Adds a building to a user")]
        [Help("This is some useless help message.")]
        public async Task AddBuilding(
            RorUser user,
            [Autocomplete(typeof(BuildingAutocompleteHandler))] Building building)
        {
            user.Player.Buildings.TryAdd(building.Name, building.BonusChance);
            _dbService.UpsertDocument("users", Builders<RorUser>.Filter.Eq(u => u.Discord.Id, Context.User.Id), user);

            await RespondAsync(
                $"Added `{building.Name}` to {user.Discord.Mention}'s list of buildings.\n" +
                $"{user.Discord.Mention} now has the following buildings: {string.Join(", ", user.Player.Buildings)}");
        }
    }
}
