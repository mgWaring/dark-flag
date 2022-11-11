using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public int playerCount;
    public RaceTimer raceTimer;
    public TextMeshProUGUI countdownText;
    public GameObject scoreboard;
    public Camera playerCam;
    public SpeedUI speedUI;
    public DurabilityUI durabilityUI;
    public LapTimer lapTimer;
    public int lapCount = 1;
    public ShipsScriptable shipScriptable;
    [HideInInspector] public string state;
    [HideInInspector] public Racer[] racers;
    [HideInInspector] public Dictionary<Racer, List<float>> laps = new Dictionary<Racer, List<float>>();

    float countdownTimer = 5.0f;
    float postRaceTimer = 10.0f;
    public int playerId = 0;
    [HideInInspector] public Racer playerRacer;
    GameObject checkpoints;
    Transform startingPositions;
    Map mMap;
    public MapScriptable map;

    void Start()
    {
        if (CrossScene.cameFromMainMenu) {
            playerCount = CrossScene.racerInfo.Length;
            map = CrossScene.map;
            lapCount = CrossScene.laps;
        } else {
            CrossScene.racerInfo = new RacerInfo[playerCount];
            if (playerId < playerCount) {
                CrossScene.racerInfo[playerId] = new RacerInfo(PlayerPrefs.GetString("playerName"), shipScriptable);
            }

            ShipsScriptable[] available = new ShipsScriptable[] { shipScriptable };
            for (int i = 0; i < playerCount; i++) {
                if (CrossScene.racerInfo[i] == null) {
                    CrossScene.racerInfo[i] = new RacerInfo(available);
                }
            }
        }

        GameObject mapObj = Instantiate(map.prefab);
        mMap = mapObj.GetComponent<Map>();
        checkpoints = mMap.checkpoints;
        startingPositions = mMap.startingPositions;
        state = "prerace";
        racers = new Racer[playerCount];
        int checkpointCount = checkpoints.transform.childCount;
        Checkpoint lastCheck = checkpoints.transform.GetChild(checkpointCount - 1).GetComponent<Checkpoint>();
        Checkpoint nextCheck = checkpoints.transform.GetChild(0).GetComponent<Checkpoint>();
        for(int i = 0; i < playerCount; i++) {
            RacerInfo info = CrossScene.racerInfo[i];
            Transform startPos = startingPositions.GetChild(i).transform;
            Vector3 pos = startPos.position;
            Quaternion rot = startPos.rotation;
            GameObject newship = Instantiate(info.ship.shipModel);
            Racer racer = newship.GetComponent<Racer>();
            MovementController mc = newship.GetComponentInChildren<MovementController>();
            mc.enabled = false;
            if (info.isBot) {
                PlayerMovement pm = newship.GetComponent<PlayerMovement>();
                pm.enabled = false;
            } else {
                BotMovement bot = newship.GetComponent<BotMovement>();
                bot.enabled = false;
                speedUI.target = mc.gameObject;
                durabilityUI.target = mc.gameObject;
                playerRacer = racer;
                playerId = i;
            }
            racer.id = i;
            racer.name = info.name;
            racer.lastCheckpoint = lastCheck;
            racer.nextCheckpoint = nextCheck;
            newship.transform.position = pos;
            newship.transform.rotation = rot;
            racers[i] = racer;
            laps[racer] = new List<float>();
        }
        if (playerRacer == null) {
            playerRacer = racers[0];
            playerId = 0;
            MovementController mc = playerRacer.gameObject.GetComponentInChildren<MovementController>();
            speedUI.target = mc.gameObject;
            durabilityUI.target = mc.gameObject;
        }

        playerCam.gameObject.GetComponent<Animator>().Play(map.cameraClipName);
    }

    void AttachCamera() {
        Transform first = playerRacer.transform;
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
        if (state == "prerace") {
            handlePreRace();
        } else if (state == "countdown") {
            updatePositions();
            handleCountdown();
        } else if (state == "race") {
            updatePositions();
            handleRace();
        } else if (state == "postrace") {
            handlePostRace();
        }
    }

    void handlePreRace() {
        Animator anim = playerCam.gameObject.GetComponent<Animator>();
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f) {
            anim.enabled = false;
            AttachCamera();
            state = "countdown";
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

    void handleCountdown() {
        countdownTimer -= Time.deltaTime;
        if (countdownTimer <= 0.0f) {
            state = "race";
            raceTimer.running = true;
            lapTimer.running = true;
            countdownText.SetText("ACTIVATE!");
            for (int i = 0; i < playerCount; i++) {
                MovementController mc = racers[i].gameObject.GetComponentInChildren<MovementController>();
                mc.enabled = true;
            }
        } else if (countdownTimer <= 1.0f) {
            countdownText.SetText("1");
        } else if (countdownTimer <= 2.0f) {
            countdownText.SetText("2");
        } else if (countdownTimer <= 3.0f) {
            countdownText.SetText("3");
        } else if (countdownTimer <= 4.0f) {
            countdownText.SetText("4");
        } else if (countdownTimer <= 5.0f) {
            countdownText.SetText("5");
        }
    }

    void handleRace() {
        if (countdownTimer >= -1.0f) {
            countdownTimer -= Time.deltaTime;
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

    public float bestLapFor(Racer racer) {
        if (laps[racer].Count == 1) {
            return 0.0f;
        } else {
            return laps[racer].GetRange(1, laps[racer].Count - 1).Min();
        }
    }
}
