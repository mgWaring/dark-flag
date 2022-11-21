using RelaySystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        public Camera camera;
        public RawImage shipImage;
        int index;

        [SerializeField] private ShipSelector _shipSelector;

        private void OnEnable() {
            _shipSelector.OnShipChange += SetPlayerShip;
        }

        private void OnDisable() {
            _shipSelector.OnShipChange -= SetPlayerShip;
        }

        private void Start() {
            nameInput.text = Player.playerName.Value;
            // when the input is changed update the player object with the new name
            nameInput.onValueChanged.AddListener(SetPlayerName);
            index = transform.GetSiblingIndex();
            transform.GetChild(0).transform.position += new Vector3(1000.0f * index, 1000.0f * index, 1000.0f * index);
            RenderTexture outputTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
            camera.targetTexture = outputTexture;
            shipImage.texture = outputTexture;
        }

        private void Update() {
            if (Player.shipChanged) {
                _shipSelector.index = Player._selectedShipIndex.Value;
                _shipSelector.SetValues();
                Player.shipChanged = false;
            }
        }

        private void PopulateAnthemsList() {
            
        }

        private void SetPlayerName(string playerName) {
            Player.playerName.Value = playerName;
        }

        private void SetPlayerShip(int shipIndex) {
            Player._selectedShipIndex.Value = shipIndex;
        }

        private void SetPlayerColour() {
            
        }

        private void SetPlayerAnthem() {
            
        }
    }
}