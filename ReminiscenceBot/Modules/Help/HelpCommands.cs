using Discord.Interactions;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ReminiscenceBot.Modules
{
    [Group("help", "Commands containing helpful information about other commands.")]
    public class HelpCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly InteractionService _commands;

        public HelpCommands(InteractionService commands)
        {
            _commands = commands;
        }

        [SlashCommand("help", "Displays an helpful explaination for a specific command.")]
        public async Task Help(string cmd)
        {
            foreach (var method in Assembly.GetExecutingAssembly().GetTypes().SelectMany(t => t.GetMethods()))
            {
                if (method.GetCustomAttribute<SlashCommandAttribute>(false) is SlashCommandAttribute ssa && ssa.Name == cmd)
                {
                    var embedBuilder = new EmbedBuilder()
                        .WithTitle(ssa.Name);
                    if (method.GetCustomAttribute<HelpAttribute>(false) is HelpAttribute ha)
                    {
                        embedBuilder.WithDescription(ha.Help);
                    }
                    await RespondAsync(embed: embedBuilder.Build());
                    return;
                }
            }

            await RespondAsync("Could not find the specified command :(");
        }

        [SlashCommand("list", "Lists all available commands with a description")]
        public async Task Help()
        {
            var cmds = _commands.SlashCommands;
            var embedBuilder = new EmbedBuilder();

            foreach (SlashCommandInfo cmd in cmds)
            {
                if (cmd.Name == "help") continue;
                embedBuilder.AddField(cmd.Name, cmd.Description);
            }

            await RespondAsync("Here's a list of commands and their description:", embed: embedBuilder.Build());
        }
    }
}
