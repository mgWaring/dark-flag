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
      for (int i = 0; i < transform.childCount; i++) {
        if (i != 0 && i != 1) {
          GameObject go = transform.GetChild(i).gameObject;
          Destroy(go);
        }
      }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
