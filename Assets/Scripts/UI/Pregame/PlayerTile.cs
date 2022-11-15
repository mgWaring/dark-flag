using RelaySystem.Data;
using TMPro;
using UnityEngine;

/*
* Extend the player tile so that it can handle input to handle:
* changing ship,
* changing name,
* changing colour,
* changing anthem,
 * */
    
namespace UI.Pregame {
    public class PlayerTile: MonoBehaviour{
        public ulong pid;
        public DFPlayer Player;
        public TMP_Text nameText;

        void PopulateAnthemsList() {
            
        }
        void PopulateShipList() {
            
        }
        void SetName() {
            nameText.text = Player.playerName;
        }

        void NextShip() {
            Player.playerShipName = "next";
        }

        void PreviousShip() {
            
        }

        void SetColour() {
            
        }

        void SetAnthem() {
            
        }
    }
}