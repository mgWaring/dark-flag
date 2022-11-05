using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class LobbyUICanvas : MonoBehaviour
    {
        [SerializeField] private Button startButton;

        public void EnableGameStart(){
            if(startButton) startButton.interactable = true;
        }
        public void DisableGameStart(){
            if(startButton) startButton.interactable = false;
        }

    }
}
