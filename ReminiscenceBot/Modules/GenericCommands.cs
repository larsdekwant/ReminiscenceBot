using System.Reflection;

using Discord;
using Discord.Interactions;

using ReminiscenceBot.Services;
using ReminiscenceBot.Models;
using Discord.WebSocket;
using Discord.Commands;

namespace ReminiscenceBot.Modules
{
    public class GenericCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly InteractionService _commands;

        public GenericCommands(InteractionService commands)
        {
            _commands = commands;
        }

        [SlashCommand("ping", "Send the bot a ping!")]
        public async Task Ping()
        {
            await RespondAsync($"Server responded in {Context.Client.Latency}ms");
        }        
    }
}
