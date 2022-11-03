using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceTimer : MonoBehaviour
{
    public TextMeshProUGUI text;
    [HideInInspector] public bool running;
    [HideInInspector] public float currentTime;

    void Start()
    {
        running = false;
        currentTime = 0.0f;
    }

    void Update()
    {
        if (running) { 
            currentTime += Time.deltaTime;
            text.text = string.Format("{0:N2}", currentTime);
        }
    }
}
