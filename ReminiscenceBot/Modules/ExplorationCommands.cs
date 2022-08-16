using Discord.Interactions;

namespace ReminiscenceBot.Modules
{
    public class ExplorationCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping", "Send the bot a ping!")]
        public async Task Ping()
        {
            await RespondAsync($"Server responded in {Context.Client.Latency}ms");
        }
    }
}
