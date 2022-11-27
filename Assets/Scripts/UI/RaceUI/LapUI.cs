using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LapUI : MonoBehaviour
{
    public GameController gameController;
    TextMeshProUGUI text;
    int laps;

    void Start() {
        text = GetComponent<TextMeshProUGUI>();
        laps = gameController.lapCount;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = string.Format("Lap: {0}/{1}", gameController.playerRacer.lap, laps);
    }
}
