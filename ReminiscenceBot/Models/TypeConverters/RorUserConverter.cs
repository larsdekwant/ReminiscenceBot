using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using MongoDB.Driver;
using ReminiscenceBot.Models;
using ReminiscenceBot.Services;

namespace ReminiscenceBot.Models
{
    /// <summary>
    /// Allows for the use of <see cref="RorUser"/> in SlashCommand parameters.
    /// The input is of type <see cref="IUser"/> and is checked against the database to see if that user has an account.
    /// If an account is found, the corresponding <see cref="RorUser"/> object is returned, otherwise a conversion error is raised.
    /// </summary>
    internal sealed class RorUserConverter : TypeConverter<RorUser>
    {
        private readonly DatabaseService _dbService;

        public RorUserConverter(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public override ApplicationCommandOptionType GetDiscordType()
        {
            return ApplicationCommandOptionType.User;
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            IUser user = (IUser)option.Value;
            RorUser? rorUser = _dbService.LoadDocuments("users", Builders<RorUser>.Filter.Eq(u => u.Discord.Id, user.Id)).FirstOrDefault();

            return rorUser is not null
                ? Task.FromResult(TypeConverterResult.FromSuccess(rorUser))
                : Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed,
                    $"Failed to convert parameter: {option.Name}\n" +
                    $"Reason: {user.Mention} does not have a Realms of Reminiscence account"));
        }
    }
}
