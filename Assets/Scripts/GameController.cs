using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;
using Managers;
using RelaySystem.Data;

public class GameController : NetworkBehaviour
{
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

  private void Start()
  {
    state = "prerace";
    if (SpawnManager.Instance.GetClientId() == 0)
    {
      var dfPlayers = GameObject.FindObjectsOfType<DFPlayer>();
      DoBasicSetup();
      _players = new List<Player>();
      _bots = new List<Bot>();
      Debug.Log("SPAWNING MAP");
      var mapObj = Instantiate(map.prefab);
      mapObj.GetComponent<NetworkObject>().Spawn();
      _mMap = mapObj.GetComponent<Map>();
      _checkpoints = _mMap.checkpoints;
      _startingPositions = _mMap.startingPositions;
      racers = new Racer[playerCount];
      var checkpointCount = _checkpoints.transform.childCount;
      var lastCheck = _checkpoints.transform.GetChild(checkpointCount - 1).GetComponent<Checkpoint>();
      var nextCheck = _checkpoints.transform.GetChild(0).GetComponent<Checkpoint>();
      for (var i = 0; i < playerCount; i++)
      {
        Debug.Log("creating players");
        var info = CrossScene.racerInfo[i];
        var startPos = _startingPositions.GetChild(i).transform;
        var pos = startPos.position;
        var rot = startPos.rotation;
        Racer racer;
        if (info.IsBot)
        {
          var botGo = Instantiate(botPrefab);
          botGo.GetComponent<NetworkObject>().Spawn();
          var bot = botGo.GetComponent<Bot>();
          _bots.Add(bot);
          bot.ss = info.Ship;
          bot.Init();
          racer = bot.racer;
          bot.SetPosRot(pos, rot);
          if (i == 0)
          {
            _firstCamAnim = bot.camera.GetComponent<Animator>();
          }
        }
        else
        {
          Debug.Log("Creating a player");
          var playerGo = Instantiate(playerPrefab);
          var dfplayer = dfPlayers.Single(dfp => dfp.OwnerClientId == info.ClientId);
          var player = playerGo.GetComponent<Player>();
          player.clientId = info.ClientId;
          playerGo.GetComponent<NetworkObject>().SpawnWithOwnership(info.ClientId);
          playerGo.transform.SetParent(dfplayer.transform, true);
          _players.Add(player);
          Debug.Log(player);
          player.ss = info.Ship;
          player.Init();
          racer = player.racer;
          playerRacer = racer;
          player.SetPosRot(pos, rot);
          if (i == 0)
          {
            _firstCamAnim = player.camera.GetComponent<Animator>();
          }
          //this needs to worked into the block above
          if (info.IsNetworkPlayer)
          {
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
    }
    else
    {
      racers = GameObject.FindObjectsOfType<Racer>();
      _players = GameObject.FindObjectsOfType<Player>().ToList();
      _mMap = GameObject.FindObjectsOfType<Map>()[0];
      _checkpoints = _mMap.checkpoints;
      var checkpointCount = _checkpoints.transform.childCount;
      var lastCheck = _checkpoints.transform.GetChild(checkpointCount - 1).GetComponent<Checkpoint>();
      var nextCheck = _checkpoints.transform.GetChild(0).GetComponent<Checkpoint>();
      for (int i = 0; i < _players.Count; i++)
      {
        _players[i].Init();
        _players[i].racer.lastCheckpoint = lastCheck;
        _players[i].racer.nextCheckpoint = nextCheck;
        laps[_players[i].racer] = new List<float>();
      }
      for (int i = 0; i < checkpointCount; i++)
      {
        _checkpoints.transform.GetChild(i).GetComponent<Checkpoint>().gameController = this;
      }
    }

    player = _players.Single(p => p.IsOwner);
    player.ship.GetComponent<BotMovement>().enabled = false;
    SetUITarget();
    PlayOpening();

    if (straightToRace)
    {
      AttachCamera();
      AllowPlay();
      raceTimer.running = true;
      lapTimer.running = true;
      state = "race";
    }
  }

  private void SetUITarget()
  {
    playerRacer = player.racer;
    MovementController mc = player.mc;

    var o = mc.gameObject;
    speedUI.SetTarget(o);
    durabilityUI.target = o;

    var cameras = GameObject.FindObjectsOfType<Camera>();
    for (int i = 0; i < cameras.Length; i++)
    {
      cameras[i].enabled = false;
    }

    player.camera.enabled = true;
  }

  private void PlayOpening()
  {
    _firstCamAnim = player.camera.GetComponent<Animator>();
    player.PlayOpening("JanktownOpeningCamera");
  }

  private void AttachCamera()
  {
    for (int i = 0; i < _players.Count; i++)
    {
      _players[i].AttachCamera();
    }
    if (_bots != null)
    {
      for (int i = 0; i < _bots.Count; i++)
      {
        _bots[i].AttachCamera();
      }
    }
  }

  private void AllowPlay()
  {

    player.AllowPlay();

    if (_bots != null)
    {
      for (int i = 0; i < _bots.Count; i++)
      {
        _bots[i].AllowPlay();
      }
    }
  }

  private void Update()
  {
    Debug.Log($"DONKEY:::::{player.ship.GetComponent<BotMovement>().enabled}");
    if (state == "prerace")
    {
      HandlePreRace();
    }
    else if (state == "countdown")
    {
      Debug.Log("COUNTDOWN");
      UpdatePositions();
      HandleCountdown();
    }
    else if (state == "race")
    {
      UpdatePositions();
      HandleRace();
    }
    else if (state == "postrace")
    {
      HandlePostRace();
    }
  }

  private void HandlePreRace()
  {
    if (_firstCamAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
    {
      Debug.Log("ATTACHING THE CAM");
      AttachCamera();
      state = "countdown";
    }
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

  private float FinishFor(Racer racer)
  {
    if (laps[racer].Count > lapCount)
    {
      return laps[racer].Last();
    }
    else
    {
      return 0.0f;
    }
  }

  public void RegisterLap(Racer racer)
  {
    var times = laps[racer];
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

    if (laps.Values.All(l => l.Count > lapCount))
    {
      state = "postrace";
      raceTimer.running = false;
      lapTimer.running = false;
      scoreboard.SetActive(true);
    }
  }

  public int PositionFor(Racer racer)
  {
    return System.Array.IndexOf(racers, racer);
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
    if (_countdownTimer >= -1.0f)
    {
      _countdownTimer -= Time.deltaTime;
    }
    else
    {
      countdownText.SetText("");
    }
  }

  private void HandlePostRace()
  {
    if (_postRaceTimer > 0.0)
    {
      _postRaceTimer -= Time.deltaTime;
    }
    else
    {
      SceneManager.LoadScene(0);
    }
  }

  public float BestLapFor(Racer racer)
  {
    if (laps[racer].Count == 1)
    {
      return 0.0f;
    }
    else
    {
      return laps[racer].GetRange(1, laps[racer].Count - 1).Min();
    }
  }
}