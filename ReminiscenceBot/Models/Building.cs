using Discord;
using Discord.Interactions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics;
using System.Security.Claims;

namespace ReminiscenceBot.Models
{
    public record Building
    {
        [BsonId, BsonIgnoreIfDefault]
        private ObjectId Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public double BonusChance { get; set; }
        public BuildingLimiter Limiter { get; set; }
        public int Limit { get; set; }

        [ComplexParameterCtor]
        public Building(
            [Summary(description: "The name of the building")] string name,
            [Summary(description: "A short description of the building")] string description,
            [Summary(description: "The additional chance this building grants in expeditions")] double bonusChance,
            [Summary(description: "What to limit the buildings to")] BuildingLimiter limiter,
            [Summary(description: "The building limit given the limiting factor")] int limit = 1)
        {
            Name = name;
            Description = description;
            BonusChance = bonusChance;
            Limiter = limiter;
            Limit = limit;
        }

        public enum BuildingLimiter
        {
            Unlimited,
            PerState,
            PerHarbour,
            PerTwoMaster
        }
    }
}
