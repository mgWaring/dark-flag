using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class InGameMenu : MonoBehaviour
{
    GameObject child;
    public InputAction menuInput;

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    void OnEnable()
    {
        menuInput.Enable();
    }

    void OnDisable()
    {
        menuInput.Disable();
    }

    void Start() {
        child = transform.GetChild(0).gameObject;
    }

    void Update() {
        if (menuInput.triggered) {
            child.SetActive(!child.activeSelf);
        }
    }
}
