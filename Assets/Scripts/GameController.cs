using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public GameObject ship;
    public int playerCount;
    public Transform startingPositions;
    public RaceTimer raceTimer;
    public TextMeshProUGUI countdownText;
    public GameObject checkpoints;
    public GameObject scoreboard;
    [HideInInspector] public string state;
    [HideInInspector] public Racer[] racers;
    [HideInInspector] public Dictionary<Racer, float> finishes = new Dictionary<Racer, float>();

    float preRaceTimer = 5.0f;

    void Start()
    {
        state = "prerace";
        racers = new Racer[playerCount];
        for(int i = 0; i < playerCount; i++) {
            Vector3 pos = startingPositions.GetChild(i).transform.position;
            GameObject newship = Instantiate(ship);
            Racer racer = newship.GetComponent<Racer>();
            racer.id = i;
            racer.lastCheckpoint = checkpoints.transform.GetChild(0).GetComponent<Checkpoint>();
            racer.nextCheckpoint = checkpoints.transform.GetChild(1).GetComponent<Checkpoint>();
            newship.transform.position = pos;
            racers[i] = racer;
        }
    }

    void Update()
    {
        updatePositions();
        if (state == "prerace") {
            handlePreRace();
        } else {
            handleRace();
        }
    }

    void updatePositions() {
        var ordered = from r in racers orderby r.GetPosition() descending, FinishFor(r) select r;
        racers = ordered.ToArray();
    }

    float FinishFor(Racer racer) {
        if (finishes.ContainsKey(racer)) {
            return finishes[racer];
        } else {
            return 0.0f;
        }
    }

    public void registerFinish(Racer racer) {
        finishes.Add(racer, raceTimer.currentTime);
        if (finishes.Count == playerCount) {
            state = "finished";
            raceTimer.running = false;
            scoreboard.active = true;
        }
    }

    public int positionFor(Racer racer) {
        return System.Array.IndexOf(racers, racer);
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
