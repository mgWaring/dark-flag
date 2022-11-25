using UnityEngine;
using UnityEngine.SceneManagement;
using Managers;

public class MainMenu : MonoBehaviour
{
    public GameObject singleplayerOptions;
    public GameObject mainMenuOptions;

    public void SinglePlayer()
    {
        singleplayerOptions.SetActive(true);
        mainMenuOptions.SetActive(false);
    }

    void Start() {
      SpawnManager.allowSpawning = true;
    }

    public void Multiplayer() {
        SceneManager.LoadSceneAsync("MP_menu");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
