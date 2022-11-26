using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace UI {
    public class HostGameButton : MonoBehaviour
    {
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
                Destroy(GameObject.FindObjectsOfType<MainMenu>()[0].gameObject);
                SceneManager.LoadSceneAsync("Pregame");
            }
        }

        public void Start()
        {
            input.text = RandomSensibleString.GenerateString();
        }
    }
}