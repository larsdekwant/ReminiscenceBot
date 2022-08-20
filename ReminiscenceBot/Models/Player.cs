using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminiscenceBot.Models
{
    public class Player
    {
        public ulong DiscordId { get; set; }
        public string MinecraftUsername { get; set; } = string.Empty;
        public string CharacterName { get; set; } = string.Empty;
        public Class Class { get; set; }
        public Race Race { get; set; }
    }

    public enum Class 
    {
        Class0,
        Class1,
        Class2
    }

    public enum Race
    {
        Race0,
        Race1,
        Race2
    }
}
