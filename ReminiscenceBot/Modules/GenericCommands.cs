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

        [SlashCommand("help", "Lists all available commands with a description")]
        public async Task Help()
        {
            var cmds = _commands.SlashCommands;
            var embedBuilder = new EmbedBuilder();

            foreach (SlashCommandInfo cmd in cmds)
            {
                if (cmd.Name == "help") continue;
                embedBuilder.AddField(cmd.Name, cmd.Description);
            }

            await RespondAsync("Here's a list of commands and their description: ", embed: embedBuilder.Build());
        }
    }
}
