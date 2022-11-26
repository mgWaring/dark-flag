using Managers;
using RelaySystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class JoinGameButton : MonoBehaviour {
        [SerializeField]
        private TMP_InputField input;

        public async void JoinLobby(){
            // won't we need to supply some player data here?
            var connected = await RelayManager.Instance.JoinGame(input.text);
        
            if (connected) {
                Destroy(GameObject.FindObjectsOfType<MainMenu>()[0].gameObject);
                SceneManager.LoadSceneAsync("Pregame");
            }
        }
    }
}
