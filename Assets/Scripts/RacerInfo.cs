using System;
using System.Collections;
using System.Collections.Generic;

public class RacerInfo
{
    public string name { get; set; }
    public ShipsScriptable ship { get; set; }
    public bool isBot { get; set; }

    public RacerInfo(string iName, ShipsScriptable iShip, bool iIsBot) {
        name = iName;
        ship = iShip;
        isBot = iIsBot;
    }

    public RacerInfo(string iName, ShipsScriptable iShip) {
        name = iName;
        ship = iShip;
        isBot = false;
    }

    /*
    public RacerInfo() {
        name = generateFakeName();
        ship = "testShip";
        isBot = true;
    }
    */

    public RacerInfo(ShipsScriptable[] ships) {
        name = generateFakeName();
        var rand = new Random();
        ship = ships[rand.Next(ships.Length)];
        isBot = true;
    }
    

    // totally necessary
    string generateFakeName() {
        string[] names = {
            "Abe",
            "Bart",
            "Camembert",
            "Doogal",
            "Ephie",
            "French",
            "Geoffrey",
            "Harriet",
            "Isiah",
            "Jonah",
            "FuckingKevin",
            "Lionel",
            "Mary",
            "Nana",
            "Orthello",
            "Pam",
            "Quintus",
            "Ruaidhrigh",
            "Stephen",
            "Tarquin",
            "Uriah",
            "Vincent",
            "Wesley",
            "Xena",
            "Yuna",
            "Zander"
        };
        var rand = new Random();
        return string.Format("{0}Bot", names[rand.Next(names.Length)]);
    }
}
