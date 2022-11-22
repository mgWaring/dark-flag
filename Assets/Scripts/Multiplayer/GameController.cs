using System.Collections.Generic;
using System.Linq;
using Managers;
using RelaySystem.Data;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer {
  public class GameController : NetworkBehaviour
  {
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
    [HideInInspector] public Racer playerRacer;
    private GameObject _checkpoints;
    private Transform _startingPositions;
    private Map _mMap;
    public MapScriptable map;
    public GameObject playerPrefab;
    public GameObject botPrefab;
    private Animator _firstCamAnim;
    Player player;

    private void DoBasicSetup()
    {
      if (CrossScene.cameFromMainMenu)
      {
        playerCount = CrossScene.racerInfo.Length;
        Debug.Log($"There are {playerCount} players");
        map = CrossScene.map;
        lapCount = CrossScene.laps;
        straightToRace = false;
      }
      else
      {
        Debug.Log("DIDNT COME FROM MENU IN MULTIPLAYER");
      }
    }

    private void Start() {
      state = "prerace";
      if (SpawnManager.Instance.GetClientId() == 0) {
        DoBasicSetup();
        SetUpMap();
        _players = new List<Player>();
        _bots = new List<Bot>();
        racers = new Racer[playerCount];
        int checkpointCount = _checkpoints.transform.childCount;
        Checkpoint lastCheck = _checkpoints.transform.GetChild(checkpointCount - 1).GetComponent<Checkpoint>();
        Checkpoint nextCheck = _checkpoints.transform.GetChild(0).GetComponent<Checkpoint>();
        for (int i = 0; i < playerCount; i++) {
          Debug.Log("creating players");
          RacerInfo info = CrossScene.racerInfo[i];
          Transform startPos = _startingPositions.GetChild(i).transform;
          Racer racer = info.IsBot
            ? CreateBot(i, info, startPos.position, startPos.rotation)
            : CreateRacer(i, info, startPos.position, startPos.rotation);

          racer.id = i;
          racer.name = info.Name;
          racer.lastCheckpoint = lastCheck;
          racer.nextCheckpoint = nextCheck;
          racers[i] = racer;
          laps[racer] = new List<float>();
        }
      } else {
        racers = FindObjectsOfType<Racer>();
        _players = FindObjectsOfType<Player>().ToList();
        _mMap = FindObjectsOfType<Map>()[0];
        _checkpoints = _mMap.checkpoints;
        int checkpointCount = _checkpoints.transform.childCount;
        Checkpoint lastCheck = _checkpoints.transform.GetChild(checkpointCount - 1).GetComponent<Checkpoint>();
        Checkpoint nextCheck = _checkpoints.transform.GetChild(0).GetComponent<Checkpoint>();
        foreach (Player player in _players) {
          player.Init();
          player.racer.lastCheckpoint = lastCheck;
          player.racer.nextCheckpoint = nextCheck;
          laps[player.racer] = new List<float>();
        }

        for (int i = 0; i < checkpointCount; i++)
          _checkpoints.transform.GetChild(i).GetComponent<Checkpoint>().gameController = this;
      }

      player = _players.Single(p => p.IsOwner);
      player.ship.GetComponent<BotMovement>().enabled = false;
      SetUITarget();
      PlayOpening();

      if (!straightToRace) return;
      AttachCamera();
      AllowPlay();
      raceTimer.running = true;
      lapTimer.running = true;
      state = "race";
    }

    private void SetUpMap() {
      Debug.Log("SPAWNING MAP");
      var mapObj = Instantiate(map.prefab);
      mapObj.GetComponent<NetworkObject>().Spawn();
      _mMap = mapObj.GetComponent<Map>();
      _checkpoints = _mMap.checkpoints;
      _startingPositions = _mMap.startingPositions;
    }

    private Racer CreateRacer(int index, RacerInfo info, Vector3 pos, Quaternion rot) {
      Debug.Log("Creating a player");
      DFPlayer[] dfPlayers = FindObjectsOfType<DFPlayer>();
      DFPlayer dfplayer = dfPlayers.Single(dfp => dfp.OwnerClientId == info.ClientId);
      var playerGo = Instantiate(playerPrefab, dfplayer.transform, true);
      Player player = playerGo.GetComponent<Player>();
      player.clientId = info.ClientId;
      playerGo.GetComponent<NetworkObject>().SpawnWithOwnership(info.ClientId);
      _players.Add(player);
      Debug.Log(player);
      player.ss = info.Ship;
      player.Init();
      playerRacer = player.racer;
      player.SetPosRot(pos, rot);
      if (index == 0) _firstCamAnim = player.camera.GetComponent<Animator>();

      return player.racer;
    }

    private Racer CreateBot(int index, RacerInfo info, Vector3 pos, Quaternion rot) {
      var botGo = Instantiate(botPrefab);
      botGo.GetComponent<NetworkObject>().Spawn();
      Bot bot = botGo.GetComponent<Bot>();
      _bots.Add(bot);
      bot.ss = info.Ship;
      bot.Init();
      bot.SetPosRot(pos, rot);
      if (index == 0) _firstCamAnim = bot.camera.GetComponent<Animator>();

      return bot.racer;
    }

    private void SetUITarget()
    {
      playerRacer = player.racer;
      MovementController mc = player.mc;

      var o = mc.gameObject;
      speedUI.SetTarget(o);
      durabilityUI.target = o;

      var cameras = GameObject.FindObjectsOfType<Camera>();
      foreach (Camera cam in cameras) {
        cam.enabled = false;
      }

      player.camera.enabled = true;
    }

    private void PlayOpening()
    {
      _firstCamAnim = player.camera.GetComponent<Animator>();
      player.PlayOpening("JanktownOpeningCamera");
    }

    private void AttachCamera() {
      foreach (var player in _players) player.AttachCamera();

      if (_bots == null) return;
    
      foreach (Bot bot in _bots) bot.AttachCamera();
    }

    private void AllowPlay()
    {
      player.AllowPlay();

      if (_bots == null) return;
    
      foreach (Bot bot in _bots) bot.AllowPlay();
    }

    private void Update() {
      Debug.Log($"DONKEY:::::{player.ship.GetComponent<BotMovement>().enabled}");
      switch (state) {
        case "prerace":
          HandlePreRace();
          break;
        case "countdown":
          Debug.Log("COUNTDOWN");
          UpdatePositions();
          HandleCountdown();
          break;
        case "race":
          UpdatePositions();
          HandleRace();
          break;
        case "postrace":
          HandlePostRace();
          break;
      }
    }

    private void HandlePreRace() {
      if (!(_firstCamAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)) return;
    
      Debug.Log("ATTACHING THE CAM");
      AttachCamera();
      state = "countdown";
    }

    private void UpdatePositions()
    {
      var ordered = from r in racers orderby r.GetPosition() descending, FinishFor(r) select r;
      racers = ordered.ToArray();
    }

    public bool RacerIsFinished(Racer racer)
    {
      return laps[racer].Count > lapCount;
    }

    private float FinishFor(Racer racer) {
      return laps[racer].Count > lapCount 
        ? laps[racer].Last() 
        : 0.0f;
    }

    public void RegisterLap(Racer racer)
    {
      List<float> times = laps[racer];
      if (times.Count == 0)
      {
        times.Add(0.0f);
      }
      else if (times.Count <= lapCount)
      {
        times.Add(raceTimer.currentTime);
        racer.lap += 1;
        if (racer.IsOwner)
        {
          lapTimer.Lap();
          if (RacerIsFinished(racer))
          {
            scoreboard.SetActive(true);
            racer.gameObject.GetComponent<BotMovement>().enabled = true;
            racer.gameObject.GetComponent<PlayerMovement>().enabled = false;
          }
        }
      }

      if (!laps.Values.All(l => l.Count > lapCount)) return;
    
      state = "postrace";
      raceTimer.running = false;
      lapTimer.running = false;
      scoreboard.SetActive(true);
    }
  
    public int PositionForPlayer()
    {
      return System.Array.IndexOf(racers, playerRacer);
    }

    private void HandleCountdown()
    {
      _countdownTimer -= Time.deltaTime;
      if (_countdownTimer <= 0.0f)
      {
        state = "race";
        raceTimer.running = true;
        lapTimer.running = true;
        countdownText.SetText("ACTIVATE!");
        Debug.Log("COUNTDOWN DONE");
        AllowPlay();
      }
      else
      {
        countdownText.SetText(Mathf.Ceil(_countdownTimer).ToString());
      }
    }

    private void HandleRace()
    {
      if (_countdownTimer >= -1.0f) _countdownTimer -= Time.deltaTime;
      else countdownText.SetText("");
    }

    private void HandlePostRace()
    {
      if (_postRaceTimer > 0.0) _postRaceTimer -= Time.deltaTime;
      else SceneManager.LoadScene(0); //todo - call networking scene manager if we're in MP
    }

    public float BestLapFor(Racer racer) {
      return laps[racer].Count == 1 
        ? 0.0f 
        : laps[racer].GetRange(1, laps[racer].Count - 1).Min();
    }
  }
}