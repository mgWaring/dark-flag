using System.Collections.Generic;
using System.Linq;
using Managers;
using RelaySystem.Data;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer
{
  public class MultiplayerGameController : NetworkBehaviour
  {
    public GameObject uiHolder;
    public int playerCount;
    public RaceTimer raceTimer;
    public TextMeshProUGUI countdownText;
    public GameObject scoreboard;
    public SpeedUI speedUI;
    public DurabilityUI durabilityUI;
    public MultiplayerAmmoUI ammoUI;
    public LapTimer lapTimer;
    public int lapCount = 1;
    public ShipsScriptable shipScriptable;
    public bool straightToRace;
    [HideInInspector] public string state;
    [HideInInspector] public MultiplayerRacer[] racers;
    public Dictionary<MultiplayerRacer, List<float>> laps = new();
    private List<MultiPlayer> _players;
    private List<MultiplayerBot> _bots;

    private float _countdownTimer = 5.0f;
    private float _postRaceTimer = 10.0f;
    [HideInInspector] public MultiplayerRacer playerRacer;
    private GameObject _checkpoints;
    private Transform _startingPositions;
    public Map _mMap;
    public MapScriptable map;
    public GameObject playerPrefab;
    public GameObject botPrefab;
    private Animator _firstCamAnim;
    MultiPlayer _multiPlayer;

    AudioClip threeSoundClip;
    AudioClip twoSoundClip;
    AudioClip oneSoundClip;
    AudioClip startSoundClip;
    AudioClip threeLapsClip;
    AudioClip twoLapsClip;
    AudioClip oneLapsClip;

    void FindAudio()
    {
      threeSoundClip = Resources.Load<AudioClip>("Audio/Voices/Countdown/three");
      twoSoundClip = Resources.Load<AudioClip>("Audio/Voices/Countdown/two");
      oneSoundClip = Resources.Load<AudioClip>("Audio/Voices/Countdown/one");
      startSoundClip = Resources.Load<AudioClip>("Audio/Voices/Countdown/engage");
      threeLapsClip= Resources.Load<AudioClip>("Audio/Voices/Laps/three");
      twoLapsClip= Resources.Load<AudioClip>("Audio/Voices/Laps/two");
      oneLapsClip= Resources.Load<AudioClip>("Audio/Voices/Laps/final");
    }

    private void DoBasicSetup()
    {
      if (CrossScene.cameFromMainMenu)
      {
        playerCount = CrossScene.racerInfo.Length;
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
      FindAudio();
      state = "prerace";
      if (SpawnManager.Instance.GetClientId() == 0)
      {
        DoBasicSetup();
        SetUpMap();
        _players = new List<MultiPlayer>();
        _bots = new List<MultiplayerBot>();
        racers = new MultiplayerRacer[playerCount];
        int checkpointCount = _checkpoints.transform.childCount;
        MultiplayerCheckpoint lastCheck = _checkpoints.transform.GetChild(checkpointCount - 1).GetComponent<MultiplayerCheckpoint>();
        MultiplayerCheckpoint nextCheck = _checkpoints.transform.GetChild(0).GetComponent<MultiplayerCheckpoint>();
        for (int i = 0; i < playerCount; i++)
        {
          RacerInfo info = CrossScene.racerInfo[i];
          Transform startPos = _startingPositions.GetChild(i).transform;
          MultiplayerRacer racer = info.IsBot
            ? CreateBot(i, info, startPos.position, startPos.rotation)
            : CreateRacer(i, info, startPos.position, startPos.rotation);

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
        lapCount = SpawnManager.Instance._lapIndex.Value + 1;
        racers = FindObjectsOfType<MultiplayerRacer>();
        _players = FindObjectsOfType<MultiPlayer>().ToList();
        _mMap = FindObjectsOfType<Map>()[0];
        _checkpoints = _mMap.checkpoints;
        int checkpointCount = _checkpoints.transform.childCount;
        MultiplayerCheckpoint lastCheck = _checkpoints.transform.GetChild(checkpointCount - 1).GetComponent<MultiplayerCheckpoint>();
        MultiplayerCheckpoint nextCheck = _checkpoints.transform.GetChild(0).GetComponent<MultiplayerCheckpoint>();
        _startingPositions = _mMap.startingPositions;
        foreach (MultiPlayer player in _players)
        {
          player.Init();
          player.racer.lastCheckpoint = lastCheck;
          player.racer.nextCheckpoint = nextCheck;
          laps[player.racer] = new List<float>();
          int index = SpawnManager.Instance.IndexFor(player.OwnerClientId);
          Transform startPos = _startingPositions.GetChild(index).transform;
          player.SetPosition(startPos.position, startPos.rotation);
          player.racer.name = SpawnManager.Instance._players[index].name.ToString();
        }

        for (int i = 0; i < checkpointCount; i++)
          _checkpoints.transform.GetChild(i).GetComponent<MultiplayerCheckpoint>().gameController = this;
      }

      _multiPlayer = _players.Single(p => p.IsOwner);
      _multiPlayer.ship.GetComponent<BotMovement>().enabled = false;
      SetUITarget();
      PlayOpening();

      if (!straightToRace) return;
      AttachCamera();
      AllowPlay();
      raceTimer.running = true;
      lapTimer.running = true;
      state = "race";
    }

    private void SetUpMap()
    {
      _checkpoints = _mMap.checkpoints;
      _startingPositions = _mMap.startingPositions;
    }

    private MultiplayerRacer CreateRacer(int index, RacerInfo info, Vector3 pos, Quaternion rot)
    {
      var playerGo = Instantiate(playerPrefab);
      MultiPlayer multiPlayer = playerGo.GetComponent<MultiPlayer>();
      playerGo.GetComponent<NetworkObject>().SpawnAsPlayerObject(info.ClientId);
      multiPlayer.clientId = info.ClientId;
      _players.Add(multiPlayer);
      multiPlayer.ss = info.Ship;
      multiPlayer.InitWithPosition(pos, rot);
      playerRacer = multiPlayer.racer;
      if (index == 0) _firstCamAnim = multiPlayer.camera.GetComponent<Animator>();

      return multiPlayer.racer;
    }

    private MultiplayerRacer CreateBot(int index, RacerInfo info, Vector3 pos, Quaternion rot)
    {
      var botGo = Instantiate(botPrefab);
      botGo.GetComponent<NetworkObject>().Spawn();
      MultiplayerBot bot = botGo.GetComponent<MultiplayerBot>();
      _bots.Add(bot);
      bot.ss = info.Ship;
      bot.Init();
      bot.SetPosRot(pos, rot);
      if (index == 0) _firstCamAnim = bot.camera.GetComponent<Animator>();

      return bot.racer;
    }

    private void SetUITarget()
    {
      playerRacer = _multiPlayer.racer;
      MovementController mc = _multiPlayer.mc;

      var o = mc.gameObject;
      speedUI.SetTarget(o);
      durabilityUI.target = o;
      ammoUI.target = o;

      var cameras = GameObject.FindObjectsOfType<Camera>();
      foreach (Camera cam in cameras)
      {
        cam.enabled = false;
      }

      _multiPlayer.camera.enabled = true;

            //Hopefule audio listener fix.
            var ears = GameObject.FindObjectsOfType<AudioListener>();
            foreach (AudioListener ear in ears)
            {
                ear.enabled = false;
            }
            _multiPlayer.camera.gameObject.GetComponent<AudioListener>().enabled = true;


        }

    private void PlayOpening()
    {
      _firstCamAnim = _multiPlayer.camera.GetComponent<Animator>();
      _multiPlayer.PlayOpening(map.cameraClipName);
    }

    private void AttachCamera()
    {
      if (map.titleClip != null) {
        GetComponent<AudioSource>().PlayOneShot(map.titleClip, 0.7f);
      }
      uiHolder.SetActive(true);
      foreach (var player in _players) player.AttachCamera();

      if (_bots == null) return;

      foreach (MultiplayerBot bot in _bots) bot.AttachCamera();
    }

    private void AllowPlay()
    {
      _multiPlayer.AllowPlay();

      if (_bots == null) return;

      foreach (MultiplayerBot bot in _bots) bot.AllowPlay();
    }

    private void Update()
    {
      switch (state)
      {
        case "prerace":
          HandlePreRace();
          break;
        case "countdown":
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

    private void HandlePreRace()
    {
      if (!(_firstCamAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)) return;

      AttachCamera();
      state = "countdown";
    }

    private void UpdatePositions()
    {
      var ordered = from r in racers orderby r.GetPosition() descending, FinishFor(r) select r;
      racers = ordered.ToArray();
    }

    public bool RacerIsFinished(MultiplayerRacer racer)
    {
      return laps[racer].Count > lapCount;
    }

    private float FinishFor(MultiplayerRacer racer)
    {
      return laps[racer].Count > lapCount
        ? laps[racer].Last()
        : 0.0f;
    }

    public void RegisterLap(MultiplayerRacer racer)
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
          int diff = lapCount - racer.lap;
          if (diff == 3) {
            GetComponent<AudioSource>().PlayOneShot(threeLapsClip, 0.7f);
          } else if (diff == 2) {
            GetComponent<AudioSource>().PlayOneShot(twoLapsClip, 0.7f);
          } else if (diff == 1) {
            GetComponent<AudioSource>().PlayOneShot(oneLapsClip, 0.7f);
          }
          lapTimer.Lap();
          if (RacerIsFinished(racer))
          {
            scoreboard.SetActive(true);
            racer.gameObject.GetComponentInParent<BotMovement>().enabled = true;
            racer.gameObject.GetComponentInParent<PlayerMovement>().enabled = false;
          }
        }
      }

      if (laps.Values.Count(l => l.Count > lapCount) < SpawnManager.Instance._players.Count) return;

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
      if (_countdownTimer <= 0.0f) {
          state = "race";
          raceTimer.running = true;
          lapTimer.running = true;
          if (countdownText.text != "ENGAGE") {
            GetComponent<AudioSource>().PlayOneShot(startSoundClip, 0.7F);
          }
          countdownText.SetText("ENGAGE");
          AllowPlay();
      } else if (_countdownTimer <= 1.0f) {
          if (countdownText.text != "1") {
            GetComponent<AudioSource>().PlayOneShot(oneSoundClip, 0.7F);
          }
          countdownText.SetText("1");
      } else if (_countdownTimer <= 2.0f) {
          if (countdownText.text != "2") {
            GetComponent<AudioSource>().PlayOneShot(twoSoundClip, 0.7F);
          }
          countdownText.SetText("2");
      } else if (_countdownTimer <= 3.0f) {
          if (countdownText.text != "3") {
            GetComponent<AudioSource>().PlayOneShot(threeSoundClip, 0.7F);
          }
          countdownText.SetText("3");
      } else if (_countdownTimer <= 4.0f) {
          countdownText.SetText("4");
      } else if (_countdownTimer <= 5.0f) {
          countdownText.SetText("5");
      }
    }

    private void HandleRace()
    {
      if (_countdownTimer >= -1.0f) _countdownTimer -= Time.deltaTime;
      else countdownText.SetText("");

      if (laps.Values.Count(l => l.Count > lapCount) < SpawnManager.Instance._players.Count) return;

      state = "postrace";
    }

    private void HandlePostRace()
    {
      if (SpawnManager.Instance.GetClientId() == 0) {
        if (_postRaceTimer > 0.0)
        {
          _postRaceTimer -= Time.deltaTime;
        } else {
          _mMap.gameObject.GetComponent<NetworkObject>().Despawn();
          Destroy(_mMap.gameObject);
          for (int i = 0; i < _players.Count; i++) {
            MultiPlayer p = _players[i];
            if (p.ship != null) {
              p.ship.GetComponent<NetworkObject>().Despawn();
              Destroy(p.ship);
              p.gameObject.GetComponent<NetworkObject>().Despawn();
              Destroy(p.gameObject);
            }
          }
          NetworkManager.Singleton.SceneManager.LoadScene("Pregame", LoadSceneMode.Single);
        }
      }
    }

    public float BestLapFor(MultiplayerRacer racer)
    {
      return laps[racer].Count == 1
        ? 0.0f
        : laps[racer].GetRange(1, laps[racer].Count - 1).Min();
    }
  }
}
