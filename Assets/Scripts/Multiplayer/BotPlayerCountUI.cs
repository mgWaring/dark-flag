using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    public class BotPlayerCountUI : MonoBehaviour {
        [SerializeField] private int maxBots = 8;
        public Selector playerSelector;
        public Selector botSelector;

        // Update is called once per frame
        private void Update() {
            var occupiedSlots = 0;
            if (playerSelector) {
                int.TryParse(playerSelector.value, out occupiedSlots);
            }

            if (NetworkManager.Singleton) {
                occupiedSlots = NetworkManager.Singleton.ConnectedClients.Count;
            }

            var bots = maxBots - occupiedSlots;
            botSelector.selection = new string[bots];
            for (var i = 0; i < (bots > 0 ? bots : 0); i++) botSelector.selection[i] = i.ToString();
            if (botSelector.value == "8") {
                botSelector.index = 7;
                botSelector.SetText();
            }
        }
    }
}