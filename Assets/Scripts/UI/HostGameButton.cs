using Managers;
using RelaySystem;
using RelaySystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace UI {
    public class HostGameButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text joinCodeText;
        [SerializeField] private TMP_InputField input;

        private void Awake()
        {
            if (!RelayManager.Instance)
                Debug.LogWarning("RelayManager not detected. Proceeding to throw toys out of pram.");
        }

        public async void HostGame()
        {
            Debug.Log("Y'all pressed the create lobby button");
            var gameUp = await RelayManager.Instance.HostGame();

            if (gameUp) {
                SceneManager.LoadSceneAsync("Pregame");
            }
        }

        public void Start()
        {
            input.text = RandomSensibleString.GenerateString();
            RelayManager.Instance.OnActiveRelayChange += UpdateActiveJoinCode;
        }

        private void UpdateActiveJoinCode(RelayHostData relayHostData)
        {
            joinCodeText.text = relayHostData.joinCode;
        }
    }
}