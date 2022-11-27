using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Multiplayer;

public class NameShower : MonoBehaviour
{
    public InputAction input;
    bool showing;

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        if (!showing && input.ReadValue<float>() > 0) {
            ShowNames();
        } else if (showing && input.ReadValue<float>() <= 0) {
            HideNames();
        }
    }

    void ShowNames()
    {
        showing = true;
        var spNames = GameObject.FindObjectsOfType<PlayerName>();
        var mpNames = GameObject.FindObjectsOfType<MultiplayerPlayerName>();
        for (int i = 0; i < spNames.Length; i++) {
            spNames[i].SetOn();
        }
        for (int i = 0; i < mpNames.Length; i++) {
            mpNames[i].SetOn();
        }
    }

    void HideNames()
    {
        showing = false;
        var spNames = GameObject.FindObjectsOfType<PlayerName>();
        var mpNames = GameObject.FindObjectsOfType<MultiplayerPlayerName>();
        for (int i = 0; i < spNames.Length; i++) {
            spNames[i].SetOff();
        }
        for (int i = 0; i < mpNames.Length; i++) {
            mpNames[i].SetOff();
        }
    }
}
