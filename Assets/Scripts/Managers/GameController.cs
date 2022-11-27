using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using UnityEditor;

public class GameController : MonoBehaviour
{
    public GameObject uiHolder;
    public int playerCount;
    public RaceTimer raceTimer;
    public TextMeshProUGUI countdownText;
    public GameObject scoreboard;
    public SpeedUI speedUI;
    public DurabilityUI durabilityUI;
    public AmmoUI ammoUI;
    public LapTimer lapTimer;
    public int lapCount = 1;
    public ShipsScriptable shipScriptable;
    public bool straightToRace = false;
    [HideInInspector] public string state;
    [HideInInspector] public Racer[] racers;
    [HideInInspector] public Dictionary<Racer, List<float>> laps = new Dictionary<Racer, List<float>>();
    List<Player> players;
    List<Bot> bots;

    float countdownTimer = 5.0f;
    float postRaceTimer = 10.0f;
    public int playerId = 0;
    [HideInInspector] public Racer playerRacer;
    GameObject checkpoints;
    Transform startingPositions;
    Map mMap;
    public MapScriptable map;
    public GameObject playerPrefab;
    public GameObject botPrefab;
    Animator firstCamAnim;

    void Start()
    {
        InputSystem.settings.SetInternalFeatureFlag("DISABLE_SHORTCUT_SUPPORT", true);
        if (CrossScene.cameFromMainMenu) {
            playerCount = CrossScene.racerInfo.Length;
            map = CrossScene.map;
            lapCount = CrossScene.laps;
            straightToRace = false;
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

        players = new List<Player>();
        bots = new List<Bot>();

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
            Racer racer;
            if (info.IsBot) {
                GameObject botGO= Instantiate(botPrefab);
                Bot bot = botGO.GetComponent<Bot>();
                bots.Add(bot);
                bot.ss = info.Ship;
                bot.Init();
                racer = bot.racer;
                bot.SetPosRot(pos, rot);
                if (i == 0) {
                    firstCamAnim = bot.camera.GetComponent<Animator>();
                }
            } else {
                GameObject playerGO = Instantiate(playerPrefab);
                Player player = playerGO.GetComponent<Player>();
                players.Add(player);
                player.ss = info.Ship;
                player.Init();
                racer = player.racer;
                playerRacer = racer;
                player.SetPosRot(pos, rot);
                if (i == 0) {
                    firstCamAnim = player.camera.GetComponent<Animator>();
                }
            }
            racer.id = i;
            racer.name = info.Name;
            racer.lastCheckpoint = lastCheck;
            racer.nextCheckpoint = nextCheck;
            racers[i] = racer;
            laps[racer] = new List<float>();
        }


        SetUITarget();
        PlayOpening();

        if (straightToRace) {
            AttachCamera();
            AllowPlay();
            raceTimer.running = true;
            lapTimer.running = true;
            state = "race";
        }
    }

    void SetUITarget() {
        playerRacer = racers[0];
        playerId = 0;
        MovementController mc;
        if (players.Count == 0) {
            mc = bots[0].mc;
        } else {
            mc = players[0].mc;
        }
        speedUI.target = mc.gameObject;
        durabilityUI.target = mc.gameObject;
        ammoUI.target = mc.gameObject;

        for (int i = 0; i < players.Count; i++) {
            if (players[i].racer.id != playerId) {
                players[i].camera.enabled = false;
            }
        }
        for (int i = 0; i < bots.Count; i++) {
            if (bots[i].racer.id != playerId) {
                bots[i].camera.enabled = false;
            }
        }
    }

    void PlayOpening() {
        for (int i = 0; i < players.Count; i++) {
            players[i].PlayOpening(map.cameraClipName);
        }
        for (int i = 0; i < bots.Count; i++) {
            bots[i].PlayOpening(map.cameraClipName);
        }
    }

    void AttachCamera() {
        uiHolder.SetActive(true);
        for (int i = 0; i < players.Count; i++) {
            players[i].AttachCamera();
        }
        for (int i = 0; i < bots.Count; i++) {
            bots[i].AttachCamera();
        }
    }

    void AllowPlay() {
        for (int i = 0; i < players.Count; i++) {
            players[i].AllowPlay();
        }
        for (int i = 0; i < bots.Count; i++) {
            bots[i].AllowPlay();
        }
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
        if (firstCamAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f) {
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
                    racer.gameObject.GetComponentInParent<PlayerMovement>().enabled = false;
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
            countdownText.SetText("ENGAGE");
            AllowPlay();
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
