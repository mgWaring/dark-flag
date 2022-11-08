using System;
using System.Collections;
using System.Collections.Generic;

public class RacerInfo
{
    public string name { get; set; }
    public string ship { get; set; }
    public bool isBot { get; set; }

    public RacerInfo(string iName, string iShip, bool iIsBot) {
        name = iName;
        ship = iShip;
        isBot = iIsBot;
    }

    public RacerInfo(string iName, string iShip) {
        name = iName;
        ship = iShip;
        isBot = false;
    }

    public RacerInfo() {
        name = generateFakeName();
        ship = "testShip";
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
