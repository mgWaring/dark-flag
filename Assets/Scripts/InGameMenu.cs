using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    GameObject child;

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    void Start() {
        child = transform.GetChild(0).gameObject;
    }

    void Update() {
        if (Input.GetKeyUp("escape")) {
            child.SetActive(!child.activeSelf);
        }
    }
}
