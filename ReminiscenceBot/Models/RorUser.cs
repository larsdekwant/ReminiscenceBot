using Discord;
using Discord.Interactions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReminiscenceBot.Models
{
    /// <summary>
    /// Represents a Realms of Reminiscence (RoR) user.
    /// </summary>
    public record RorUser
    {
        public DiscordInfo Discord { get; set; }
        public PlayerInfo Player { get; set; }

        public RorUser(DiscordInfo discord, PlayerInfo player)
        {
            Discord = discord;
            Player = player;
        }
    }

    /// <summary>
    /// Contains some generic information about the users' Discord.
    /// </summary>
    [BsonNoId]
    public record DiscordInfo
    {
        public ulong Id { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        
        public DiscordInfo(IUser user)
        {
            Id = user.Id;
            Username = user.Username;
            Discriminator = user.Discriminator;
        }

        public string Mention => $"<@{Id}>";
        public string Tag => $"{Username}#{Discriminator}";
    }

    /// <summary>
    /// Contains all information about a player on the Realms of Reminiscence (RoR) minecraft server.
    /// </summary>
    [BsonNoId]
    public record PlayerInfo
    {
        public string McUsername { get; set; }
        public string RorName { get; set; }
        public McHeroesClass Class { get; set; }
        public McRacesOfThana Race { get; set; }
        public Dictionary<string, double> Buildings { get; set; }

        [ComplexParameterCtor]
        public PlayerInfo(
            [Summary(description: "Your minecraft username")] string mcUsername,
            [Summary(description: "Your Realms of Reminiscence character name")] string rorName,
            [Summary(description: "Your characters' class")] McHeroesClass @class,
            [Summary(description: "Your characters' race")] McRacesOfThana race)
        {
            McUsername = mcUsername;
            RorName = rorName;
            Class = @class;
            Race = race;
            Buildings = new Dictionary<string, double>();
        }

        public string Character => $"{RorName} ({Race} {Class})";
    }

    /// <summary>
    /// All classes available to the players from the Heroes plugin.
    /// </summary>
    public enum McHeroesClass
    {
        Sorcerer,
        Druid,
        Pyromancer,
        Warlock,
        Cleric,
        Shaman,
        Enchanter
    }

    /// <summary>
    /// All races playable for the players.
    /// </summary>
    public enum McRacesOfThana
    {
        Human,
        Elf,
        Dwarf,
        Gnome,
        Tiefling,
        Halfling
    }
}
