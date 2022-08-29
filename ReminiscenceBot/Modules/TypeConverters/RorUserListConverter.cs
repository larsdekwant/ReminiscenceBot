using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using MongoDB.Driver;
using ReminiscenceBot.Models;
using ReminiscenceBot.Services;

namespace ReminiscenceBot.Modules.TypeConverters
{
    /// <summary>
    /// Allows for the use of a list of <see cref="RorUser>"/> in SlashCommand parameters.
    /// The input is of type <see cref="string"/> and is parsed as a list of discord user mentions.
    /// The discord ids are extracted using <see cref="Regex"/> and are then checked against the database.
    /// If all accounts are found, the corresponding list of <see cref="RorUser"/> is returned, 
    /// otherwise if any account is missing, a conversion error is raised.
    /// </summary>
    internal sealed class RorUserListConverter : TypeConverter<List<RorUser>>
    {
        private readonly DatabaseService _dbService;

        public RorUserListConverter(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public override ApplicationCommandOptionType GetDiscordType()
        {
            return ApplicationCommandOptionType.String;
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            var discordIds = Regex
                .Matches((string)option.Value, @"<@!?(?<id>\d+)>")
                .Select(m => ulong.Parse(m.Groups["id"].Value));

            List<RorUser> users = _dbService.LoadDocuments("users", Builders<RorUser>.Filter.In(x => x.Discord.Id, discordIds));

            // Create collection of users that do not have an account registered on their discord id.
            var invalidIds = discordIds.Except(users.Select(u => u.Discord.Id));

            return !invalidIds.Any()
                ? Task.FromResult(TypeConverterResult.FromSuccess(users))
                : Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed,
                    $"Failed to convert parameter: {option.Name}\n" +
                    $"Reason: The following users do not have a Realms of Reminiscence account: {string.Join(' ', invalidIds.Select(id => $"<@{id}>"))}"));
        }
    }
}
