using Discord;
using Discord.Interactions;

using ReminiscenceBot.Services;
using ReminiscenceBot.Models;

namespace ReminiscenceBot.Modules.AutocompleteHandlers
{
    /// <summary>
    /// This class handles the autocompletion for parameters of type <see cref="Continent"/>
    /// The suggestions are dynamically pulled from the database.
    /// </summary>
    public class ContinentAutocompleteHandler : AutocompleteHandler
    {
        private readonly DatabaseService _dbService;

        /// <summary>
        /// <see cref="DatabaseService"/> gets injected by Dependency Injection
        /// </summary>
        /// <param name="dbService">The service to interact with the database</param>
        public ContinentAutocompleteHandler(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services)
        {
            var continents = _dbService.LoadAllDocuments<Continent>("continents");
            var autocomplete = continents.Select(b => new AutocompleteResult(b.Name, b.Name));
            return AutocompletionResult.FromSuccess(autocomplete.Take(25));
        }
    }
}
