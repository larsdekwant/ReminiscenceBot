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

namespace ReminiscenceBot.Models.TypeConverters
{
    /// <summary>
    /// Allows for the use of <see cref="Building"/> in SlashCommand parameters.
    /// The input is of type <see cref="string"/> and is checked against the database to see if that building exists (based on name)
    /// If the building is found, the corresponding <see cref="Building"/> object is returned, otherwise a conversion error is raised.
    /// </summary>
    internal sealed class BuildingConverter : TypeConverter<Building>
    {
        private readonly DatabaseService _dbService;

        public BuildingConverter(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public override ApplicationCommandOptionType GetDiscordType()
        {
            return ApplicationCommandOptionType.String;
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            string buildingName = (string)option.Value;
            Building? building = _dbService.LoadDocuments("buildings", Builders<Building>.Filter.Eq(b => b.Name, buildingName)).FirstOrDefault();

            return building is not null
                ? Task.FromResult(TypeConverterResult.FromSuccess(building))
                : Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed,
                    $"Failed to convert parameter: {option.Name}\n" +
                    $"Reason: Building `{buildingName}` does not exist. Use `/building list` to show a list of all buildings."));
        }        
    }
}
