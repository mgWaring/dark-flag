using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuSettings : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    void Start()
    {
        if (CrossScene.playerName != "") {
            nameText.text = CrossScene.playerName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CrossScene.playerName = nameText.text;
    }
}
