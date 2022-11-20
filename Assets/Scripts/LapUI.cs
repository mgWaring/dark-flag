using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LapUI : MonoBehaviour
{
    public GameController gameController;
    private TextMeshProUGUI text;
    private int laps;

    private void Start() {
        text = GetComponent<TextMeshProUGUI>();
        laps = gameController.lapCount;
    }

    // Update is called once per frame
    private void Update()
    {
        text.text = string.Format("Lap: {0}/{1}", gameController.playerRacer.lap, laps);
    }
}
