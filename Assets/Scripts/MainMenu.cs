using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject singleplayerOptions;
    public GameObject mainMenuOptions;

    public void SinglePlayer()
    {
        singleplayerOptions.SetActive(true);
        mainMenuOptions.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Multiplayer() {
        Debug.Log("nah mate");
    }
}
