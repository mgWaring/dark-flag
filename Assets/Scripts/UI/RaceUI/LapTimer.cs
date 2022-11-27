using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LapTimer : MonoBehaviour
{
    public TextMeshProUGUI bestTimeText;
    [HideInInspector] public bool running;
    float currentTime;
    TextMeshProUGUI text;
    float best = 1000000000.0f;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        running = false;
        currentTime = 0.0f;
        bestTimeText.text = "Best: N/A";
    }

    void Update()
    {
        if (running) { 
            currentTime += Time.deltaTime;
            text.text = string.Format("{0:N2}", currentTime);
        }
    }

    public void Lap() {
        if (best != 1000000000.0f) {
            if (currentTime < best) {
                best = currentTime;
            }
        } else {
            best = currentTime;
        }
        bestTimeText.text = string.Format("Best: {0:N2}", best);
        currentTime = 0.0f;
    }
}
