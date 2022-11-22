using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer {
    public class SinglePlayerOptions : MonoBehaviour
    {
        public GameObject mainMenuOptions;
        public Selector mapSelector;
        public Selector lapSelector;
        public Selector playerSelector;
        public Selector botSelector;
        public MapScriptable[] maps;

        public void Back()
        {
            gameObject.SetActive(false);
            mainMenuOptions.SetActive(true);
        }

        public void StartGame()
        {
            int lapCount = 1;
            int playerCount = 1;
            int botCount = 0;
            int.TryParse(lapSelector.value, out lapCount);
            int.TryParse(playerSelector.value, out playerCount);
            int.TryParse(botSelector.value, out botCount);
        
            CrossScene.map = maps.Single(m => m.name == mapSelector.value);
            CrossScene.laps = lapCount;
            CrossScene.players = playerCount;
            CrossScene.bots = botCount;
            CrossScene.cameFromMainMenu = true;
            SceneManager.LoadScene(1);
        }

    }
}

