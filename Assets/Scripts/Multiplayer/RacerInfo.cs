using System;

namespace Multiplayer {
    public class RacerInfo
    {
        public string Name { get; set; }
        public ShipsScriptable Ship { get; set; }
        public bool IsBot { get; set; }
        public bool IsNetworkPlayer { get; set; }
        public ulong ClientId { get; set; }

        public RacerInfo(string name, ShipsScriptable ship, ulong clientId) {
            Name = name;
            Ship = ship;
            ClientId = clientId;
            IsNetworkPlayer = true;
        }
    
        public RacerInfo(string name, ShipsScriptable ship, bool isBot) {
            Name = name;
            Ship = ship;
            IsBot = isBot;
        }

        public RacerInfo(string name, ShipsScriptable ship) {
            Name = name;
            Ship = ship;
        }
    
        public RacerInfo(ShipsScriptable[] ships) {
            var rand = new Random();
            Name = GenerateFakeName();
            Ship = ships[rand.Next(ships.Length)];
            IsBot = true;
        }
    

        // totally necessary
        private string GenerateFakeName() {
            string[] names = {
                "Abe",
                "Bart",
                "Camenbert",
                "Dougal",
                "Ephey", // * eh he he he he *
                "French",
                "Geoffrey",
                "Harriet",
                "Isiah",
                "Jonah",
                "FUCKINGKEVIN",
                "Lionel",
                "Mary",
                "Nana",
                "Othello",
                "Pam",
                "Quintus",
                "Ruaraidh",
                "Stephen",
                "Tarquin",
                "Uriah",
                "Vincent",
                "Wesley",
                "Xena",
                "Yuna",
                "Zander",
                "Dirk Gently",
                "Zanic"
            };
            var rand = new Random();
            return $"{names[rand.Next(names.Length)]}Bot";
        }
    }
}
