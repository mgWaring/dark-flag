using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SinglePlayerOptions : MonoBehaviour
{
    public GameObject mainMenuOptions;
    public Selector mapSelector;
    public Selector lapSelector;
    public Selector playerSelector;
    public Selector botSelector;

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

        switch(mapSelector.value) {
            case "Janktown":
                CrossScene.map = GameController.MapFabName.JanktownSpeedway;
                break;
            case "Scrap Palace":
                CrossScene.map = GameController.MapFabName.ScrapPalace;
                break;
            default:
                CrossScene.map = GameController.MapFabName.JanktownSpeedway;
                break;
        }
        CrossScene.laps = lapCount;
        CrossScene.players = playerCount;
        CrossScene.bots = botCount;
        CrossScene.cameFromMainMenu = true;
        SceneManager.LoadScene(1);
    }
}

