using Discord;
using Discord.Interactions;

using ReminiscenceBot.Services;
using ReminiscenceBot.Models;
using Discord.WebSocket;
using MongoDB.Driver;
using System.Numerics;

namespace ReminiscenceBot.Modules
{
    [Group("user", "Commands related to users")]
    public class UserCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DatabaseService _dbService;

        public UserCommands(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        [SlashCommand("profile", "Displays a profile for a given user (defaults to the yourself)")]
        [Help("This is some useless help message.")]
        public async Task ShowUserProfile(IUser? user = null)
        {
            if (user == null) user = Context.User;
            RorUser? rorUser = _dbService.LoadDocuments("users", 
                Builders<RorUser>.Filter.Eq(u => u.Discord.Id, user.Id)).FirstOrDefault();            

            // Check whether an user was found in the database
            if (rorUser is null)
            {
                await RespondAsync(
                    "This user does not have a profile.\n" +
                    "Use `/user list` to show a list of all Realms of Reminiscence users.\n" +
                    "Use `/user set` to setup your own user profile.");
                return;
            }

            // Build the profile embed.
            var embedBuilder = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithTitle("Realms of Reminiscence profile")
                .WithAuthor(user)
                .AddField("Minecraft name", rorUser.Player.McUsername)
                .AddField("Character name", rorUser.Player.RorName)
                .AddField("Class", rorUser.Player.Class)
                .AddField("Race", rorUser.Player.Race)
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
        public async Task AddBuilding(IUser user, string buildingName)
        {
            RorUser? rorUser = _dbService.LoadDocuments("users",
                Builders<RorUser>.Filter.Eq(u => u.Discord.Id, user.Id)).FirstOrDefault();

            // Check whether an user was found in the database
            if (rorUser is null)
            {
                await RespondAsync(
                    "This user does not have a profile.\n" +
                    "Use `/user list` to show a list of all Realms of Reminiscence users.\n" +
                    "Use `/user set` to setup your own user profile.");
                return;
            }

            Building? building = _dbService.LoadDocuments("buildings",
                Builders<Building>.Filter.Eq(b => b.Name, buildingName)).FirstOrDefault();

            // Check whether an user was found in the database
            if (building is null)
            {
                await RespondAsync($"Building `{buildingName}` does not exist. Use `/building list` to show a list of all buildings.");
                return;
            }

            // Finally add the building to the user
            rorUser.Player.Buildings.Add(buildingName);
            _dbService.UpsertDocument("users",
                Builders<RorUser>.Filter.Eq(u => u.Discord.Id, Context.User.Id),
                rorUser);

            await RespondAsync(
                $"Added `{buildingName}` to {rorUser.Discord.Mention}'s list of buildings.\n" +
                $"{rorUser.Discord.Mention} now has the following buildings: {string.Join(", ", rorUser.Player.Buildings)}");
        }
    }
}
