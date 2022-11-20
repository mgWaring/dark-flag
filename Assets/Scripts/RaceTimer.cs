using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceTimer : MonoBehaviour
{
    [HideInInspector] public bool running;
    [HideInInspector] public float currentTime;

    private void Start()
    {
        running = false;
        currentTime = 0.0f;
    }

    private void Update()
    {
        if (running) { 
            currentTime += Time.deltaTime;
        }
    }
}
