using RelaySystem.Data;
using TMPro;
using UnityEngine;

/*
* todo: Extend the player tile so that it can handle input to handle:
* changing colour,
* changing anthem,
 * */
    
namespace UI.Pregame {
    public class PlayerTile: MonoBehaviour{

        public DFPlayer Player;
        public TMP_InputField nameInput;
        public bool ready { get; }

        [SerializeField] private ShipSelector _shipSelector;

        private void OnEnable() {
            _shipSelector.OnShipChange += SetPlayerShip;
        }

        private void OnDisable() {
            _shipSelector.OnShipChange -= SetPlayerShip;
        }

        private void Start() {
            nameInput.text = Player.playerName;
            // when the input is changed update the player object with the new name
            nameInput.onValueChanged.AddListener(SetPlayerName);
        }

        private void PopulateAnthemsList() {
            
        }

        private void SetPlayerName(string playerName) {
            Player.playerName = playerName;
        }

        private void SetPlayerShip(string shipName) {
            Player.playerShipName = shipName;
        }

        private void SetPlayerColour() {
            
        }

        private void SetPlayerAnthem() {
            
        }
    }
}