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
        SceneManager.LoadSceneAsync("MP_menu");
    }
}
