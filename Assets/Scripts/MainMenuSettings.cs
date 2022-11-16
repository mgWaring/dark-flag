using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuSettings : MonoBehaviour
{
    public TMP_InputField inputField;

    void OnEnable()
    {
        string playerName = PlayerPrefs.GetString("playerName");
        if (playerName != "") {
            inputField.text = playerName;
        };
    }

    public void SaveSettings() {
        PlayerPrefs.SetString("playerName", inputField.text);
        PlayerPrefs.Save();
    }
}
