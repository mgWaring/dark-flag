using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public GameObject ship;
    public int playerCount;
    public Transform startingPositions;
    public RaceTimer raceTimer;
    public TextMeshProUGUI countdownText;
    [HideInInspector] public string state;

    float preRaceTimer = 5.0f;
    Dictionary<string, float> finishes = new Dictionary<string, float>();

    void Start()
    {
        state = "prerace";
        for(int i = 0; i < playerCount; i++) {
            Vector3 pos = startingPositions.GetChild(i).transform.position;
            GameObject newship = Instantiate(ship);
            newship.transform.position = pos;
        }
    }

    void Update()
    {
        if (state == "prerace") {
            handlePreRace();
        } else {
            handleRace();
        }
    }

    public void registerFinish(string name) {
        finishes.Add(name, raceTimer.currentTime);
        if (finishes.Count == playerCount) {
            state = "finished";
            raceTimer.running = false;
        }
    }

    void handlePreRace() {
        preRaceTimer -= Time.deltaTime;
        if (preRaceTimer <= 0.0f) {
            state = "race";
            raceTimer.running = true;
            countdownText.SetText("ACTIVATE!");
        } else if (preRaceTimer <= 1.0f) {
            countdownText.SetText("1");
        } else if (preRaceTimer <= 2.0f) {
            countdownText.SetText("2");
        } else if (preRaceTimer <= 3.0f) {
            countdownText.SetText("3");
        }
    }

    void handleRace() {
        if (preRaceTimer >= -1.0f) {
            preRaceTimer -= Time.deltaTime;
        } else {
            countdownText.SetText("");
        }
    }
}
