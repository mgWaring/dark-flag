using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public GameObject ship;
    public int playerCount;
    public RaceTimer raceTimer;
    public TextMeshProUGUI countdownText;
    public GameObject scoreboard;
    public Camera playerCam;
    public SpeedUI speedUI;
    public LapTimer lapTimer;
    public int lapCount = 1;
    [HideInInspector] public string state;
    [HideInInspector] public Racer[] racers;
    [HideInInspector] public Dictionary<Racer, List<float>> laps = new Dictionary<Racer, List<float>>();

    float preRaceTimer = 5.0f;
    float postRaceTimer = 10.0f;
    public int playerId = 0;
    [HideInInspector] public Racer playerRacer;
    GameObject checkpoints;
    Transform startingPositions;
    Map map;
    public string mapFabName;

    void Start()
    {

        GameObject mapFab = Resources.Load<GameObject>("Prefabs/TrackJanktownSpeedway");
        GameObject mapObj = Instantiate(mapFab);
        map = mapObj.GetComponent<Map>();
        checkpoints = map.checkpoints;
        startingPositions = map.startingPositions;
        state = "prerace";
        racers = new Racer[playerCount];
        int checkpointCount = checkpoints.transform.childCount;
        Checkpoint lastCheck = checkpoints.transform.GetChild(checkpointCount - 1).GetComponent<Checkpoint>();
        Checkpoint nextCheck = checkpoints.transform.GetChild(0).GetComponent<Checkpoint>();
        for(int i = 0; i < playerCount; i++) {
            Transform startPos = startingPositions.GetChild(i).transform;
            Vector3 pos = startPos.position;
            Quaternion rot = startPos.rotation;
            GameObject newship = Instantiate(ship);
            Racer racer = newship.GetComponent<Racer>();
            MovementController mc = newship.GetComponentInChildren<MovementController>();
            mc.enabled = false;
            if (i == playerId) {
                BotMovement bot = newship.GetComponent<BotMovement>();
                bot.enabled = false;
                speedUI.target = mc.gameObject;
                playerRacer = racer;
            } else {
                PlayerMovement pm = newship.GetComponent<PlayerMovement>();
                pm.enabled = false;
            }
            racer.id = i;
            racer.lastCheckpoint = lastCheck;
            racer.nextCheckpoint = nextCheck;
            newship.transform.position = pos;
            newship.transform.rotation = rot;
            racers[i] = racer;
            laps[racer] = new List<float>();
        }
        Transform first = racers[0].transform;
        Transform camTransform = playerCam.transform;
        camTransform.SetParent(first);
        camTransform.position = first.position;
        camTransform.position += first.up * 2;
        camTransform.position -= first.forward * 3.5f;
        camTransform.rotation = first.rotation;
        camTransform.Rotate(15,0,0);
    }

    void Update()
    {
        updatePositions();
        if (state == "prerace") {
            handlePreRace();
        } else if (state == "race") {
            handleRace();
        } else if (state == "postrace") {
            handlePostRace();
        }
    }

    void updatePositions() {
        var ordered = from r in racers orderby r.GetPosition() descending, FinishFor(r) select r;
        racers = ordered.ToArray();
    }

    public bool RacerIsFinished(Racer racer) {
        return laps[racer].Count > lapCount;
    }

    float FinishFor(Racer racer) {
        if (laps[racer].Count > lapCount) {
            return laps[racer].Last();
        } else {
            return 0.0f;
        }
    }

    public void registerLap(Racer racer) {
        List<float> times = laps[racer];
        if (times.Count == 0) {
            times.Add(0.0f);
        } else if (times.Count <= lapCount) {
            times.Add(raceTimer.currentTime);
            racer.lap += 1;
            if (racer.id == playerId) {
                lapTimer.Lap();
                if (RacerIsFinished(racer)) {
                    scoreboard.SetActive(true);
                    racer.gameObject.GetComponent<BotMovement>().enabled = true;
                    racer.gameObject.GetComponent<PlayerMovement>().enabled = false;
                }
            }
        }

        if (laps.Values.All(l => l.Count > lapCount)) {
            state = "postrace";
            raceTimer.running = false;
            lapTimer.running = false;
            scoreboard.SetActive(true);
        }
    }

    public int positionFor(Racer racer) {
        return System.Array.IndexOf(racers, racer);
    }

    public int positionForPlayer() {
        return System.Array.IndexOf(racers, playerRacer);
    }

    void handlePreRace() {
        preRaceTimer -= Time.deltaTime;
        if (preRaceTimer <= 0.0f) {
            state = "race";
            raceTimer.running = true;
            lapTimer.running = true;
            countdownText.SetText("ACTIVATE!");
            for (int i = 0; i <= playerCount; i++) {
                MovementController mc = racers[i].gameObject.GetComponentInChildren<MovementController>();
                mc.enabled = true;
            }
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

    void handlePostRace() {
        if (postRaceTimer > 0.0) {
            postRaceTimer -= Time.deltaTime;
        } else {
            SceneManager.LoadScene(0);
        }
    }
}
