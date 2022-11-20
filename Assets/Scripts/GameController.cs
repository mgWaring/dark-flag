using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour {
    [SerializeField] private bool devmode;
    public int playerCount;
    public RaceTimer raceTimer;
    public TextMeshProUGUI countdownText;
    public GameObject scoreboard;
    public SpeedUI speedUI;
    public DurabilityUI durabilityUI;
    public LapTimer lapTimer;
    public int lapCount = 1;
    public ShipsScriptable shipScriptable;
    public bool straightToRace;
    [HideInInspector] public string state;
    [HideInInspector] public Racer[] racers;
    public Dictionary<Racer, List<float>> laps = new();
    private List<Player> _players;
    private List<Bot> _bots;

    private float _countdownTimer = 5.0f;
    private float _postRaceTimer = 10.0f;
    public int playerId;
    [HideInInspector] public Racer playerRacer;
    private GameObject _checkpoints;
    private Transform _startingPositions;
    private Map _mMap;
    public MapScriptable map;
    public GameObject playerPrefab;
    public GameObject botPrefab;
    private Animator _firstCamAnim;

    private void DoBasicSetup() {
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

            var available = new[] { shipScriptable };
            for (var i = 0; i < playerCount; i++) {
                CrossScene.racerInfo[i] ??= new RacerInfo(available);
            }
        }
    }

    private void Start() {
        DoBasicSetup();
        _players = new List<Player>();
        _bots = new List<Bot>();
        var mapObj = Instantiate(map.prefab);
        _mMap = mapObj.GetComponent<Map>();
        _checkpoints = _mMap.checkpoints;
        _startingPositions = _mMap.startingPositions;
        state = "prerace";
        racers = new Racer[playerCount];
        var checkpointCount = _checkpoints.transform.childCount;
        var lastCheck = _checkpoints.transform.GetChild(checkpointCount - 1).GetComponent<Checkpoint>();
        var nextCheck = _checkpoints.transform.GetChild(0).GetComponent<Checkpoint>();
        for (var i = 0; i < playerCount; i++) {
            var info = CrossScene.racerInfo[i];
            var startPos = _startingPositions.GetChild(i).transform;
            var pos = startPos.position;
            var rot = startPos.rotation;
            Racer racer;
            if (info.IsBot) {
                var botGo = Instantiate(botPrefab);
                var bot = botGo.GetComponent<Bot>();
                _bots.Add(bot);
                bot.ss = info.Ship;
                bot.Init();
                racer = bot.racer;
                bot.SetPosRot(pos, rot);
                if (i == 0) {
                    _firstCamAnim = bot.camera.GetComponent<Animator>();
                }
            } else {
                var playerGo = Instantiate(playerPrefab);
                var player = playerGo.GetComponent<Player>();
                _players.Add(player);
                Debug.Log(player);
                player.ss = info.Ship;
                player.Init();
                racer = player.racer;
                playerRacer = racer;
                player.SetPosRot(pos, rot);
                if (i == 0) {
                    _firstCamAnim = player.camera.GetComponent<Animator>();
                }
                //this needs to worked into the block above
                if (info.IsNetworkPlayer) {
                    //var relevantClient = NetworkManager.Singleton.ConnectedClients[info.ClientId];
                    //newShip.transform.parent = relevantClient.PlayerObject.transform;
                    //relevantClient.PlayerObject.GetComponent<DFPlayer>().ClaimShip(newShip);
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

    private void SetUITarget() {
        playerRacer = racers[0];
        playerId = 0;
        MovementController mc;
        if (_players.Count == 0) {
            mc = _bots[0].mc;
        } else {
            mc = _players[0].mc;
        }

        var o = mc.gameObject;
        speedUI.target = o;
        durabilityUI.target = o;

        for (int i = 0; i < _players.Count; i++) {
            if (_players[i].racer.id != playerId) {
                _players[i].camera.enabled = false;
            }
        }
        for (int i = 0; i < _bots.Count; i++) {
            if (_bots[i].racer.id != playerId) {
                _bots[i].camera.enabled = false;
            }
        }
    }

    private void PlayOpening() {
        for (int i = 0; i < _players.Count; i++) {
            _players[i].PlayOpening(map.cameraClipName);
        }
        for (int i = 0; i < _bots.Count; i++) {
            _bots[i].PlayOpening(map.cameraClipName);
        }
    }
    
    private void AttachCamera() {
        for (int i = 0; i < _players.Count; i++) {
            _players[i].AttachCamera();
        }
        for (int i = 0; i < _bots.Count; i++) {
            _bots[i].AttachCamera();
        }
    }

    private void AllowPlay() {
        for (int i = 0; i < _players.Count; i++) {
            _players[i].AllowPlay();
        }
        for (int i = 0; i < _bots.Count; i++) {
            _bots[i].AllowPlay();
        }
    }

    private void Update() {
        if (state == "prerace") {
            HandlePreRace();
        } else if (state == "countdown") {
            UpdatePositions();
            HandleCountdown();
        } else if (state == "race") {
            UpdatePositions();
            HandleRace();
        } else if (state == "postrace") {
            HandlePostRace();
        }
    }

    private void HandlePreRace() {
        if (_firstCamAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f) {
            AttachCamera();
            state = "countdown";
        }
    }

    private void UpdatePositions() {
        var ordered = from r in racers orderby r.GetPosition() descending, FinishFor(r) select r;
        racers = ordered.ToArray();
    }

    public bool RacerIsFinished(Racer racer) {
        return laps[racer].Count > lapCount;
    }

    private float FinishFor(Racer racer) {
        if (laps[racer].Count > lapCount) {
            return laps[racer].Last();
        } else {
            return 0.0f;
        }
    }

    public void RegisterLap(Racer racer) {
        var times = laps[racer];
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

    public int PositionFor(Racer racer) {
        return System.Array.IndexOf(racers, racer);
    }

    public int PositionForPlayer() {
        return System.Array.IndexOf(racers, playerRacer);
    }

    private void HandleCountdown() {
        _countdownTimer -= Time.deltaTime;
        if (_countdownTimer <= 0.0f || devmode) {
            state = "race";
            raceTimer.running = true;
            lapTimer.running = true;
            countdownText.SetText("ACTIVATE!");
            AllowPlay();
            for (var i = 0; i < playerCount; i++) {
                var mc = racers[i].gameObject.GetComponentInChildren<MovementController>();
                mc.enabled = true;
            }
        } else {
            countdownText.SetText(Mathf.Ceil(_countdownTimer).ToString());
        }
    }

    private void HandleRace() {
        if (_countdownTimer >= -1.0f) {
            _countdownTimer -= Time.deltaTime;
        } else {
            countdownText.SetText("");
        }
    }

    private void HandlePostRace() {
        if (_postRaceTimer > 0.0) {
            _postRaceTimer -= Time.deltaTime;
        } else {
            SceneManager.LoadScene(0);
        }
    }

    public float BestLapFor(Racer racer) {
        if (laps[racer].Count == 1) {
            return 0.0f;
        } else {
            return laps[racer].GetRange(1, laps[racer].Count - 1).Min();
        }
    }
}