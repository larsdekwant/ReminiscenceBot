using Discord;
using Discord.Interactions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics;
using System.Security.Claims;

namespace ReminiscenceBot.Models
{
    public record Continent : BaseMongoItem
    {
        public string Name { get; set; }
        public string Description { get; set; }        

        [ComplexParameterCtor]
        public Continent(
            [Summary(description: "The name of the continent")] string name,
            [Summary(description: "A description of the continent")] string description)
        {
            Name = name;
            Description = description;
        }        
    }
}
