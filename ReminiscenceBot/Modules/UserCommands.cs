using Discord;
using Discord.Interactions;

using ReminiscenceBot.Services;
using ReminiscenceBot.Models;
using Discord.WebSocket;
using MongoDB.Driver;

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
        public async Task ShowUserProfile(IUser? user = null)
        {
            if (user == null) user = Context.User;
            RorUser? rorUser = _dbService.LoadDocument("users", Builders<RorUser>.Filter.Eq(x => x.Discord.Id, user.Id));

            // Check whether an user was found in the database
            if (rorUser == null)
            {
                await RespondAsync("This user does not have a profile.");
                return;
            }

            // Build the profile embed.
            var embedBuilder = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithTitle("Profile for " + user.Username)
                .AddField("RoR name", rorUser.Player.RorName);
            await RespondAsync(embed: embedBuilder.Build());
        }

        [SlashCommand("set", "Set or update your user profile information.")]
        public async Task UpdateUser([ComplexParameter] PlayerInfo player)
        {
            _dbService.UpsertDocument("users",
                Builders<RorUser>.Filter.Eq(x => x.Discord.Id, Context.User.Id),
                new RorUser(new DiscordInfo(Context.User), player));

            await RespondAsync($"Updated your user profile!");
        }

        [SlashCommand("list", "Lists all users of the bot")]
        public async Task ListAllUsers()
        {
            List<RorUser> rorUsers = _dbService.LoadAllDocuments<RorUser>("users");

            var embedBuilder = new EmbedBuilder()
                .WithTitle("List of all Realms of Reminiscence users")
                .WithDescription(string.Join('\n', rorUsers.Select(rorUser => rorUser.Discord.Mention)))
                .WithCurrentTimestamp();

            await RespondAsync(embed: embedBuilder.Build());
        }
    }
}
