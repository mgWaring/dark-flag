using System.Collections.Generic;
using System.Linq;
using Managers;
using RelaySystem.Data;
using UI.Pregame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Multiplayer {
  public class MultiPlayerOptions : MonoBehaviour
  {
    public MultiplayerSelector mapSelector;
    public MultiplayerSelector lapSelector;
    public MultiplayerSelector botSelector;
    private Dictionary<string, MapScriptable> _maps = new();
    private Dictionary<string, ShipsScriptable> _ships = new();
    [SerializeField] private PlayerList playerList;
    public ShipsScriptable[] shipList;
    public MapScriptable[] mapList;

    private void Start()
    {
      FindShips();
      FindMaps();
    }

    private void FindShips()
    {
      for (int i = 0; i < shipList.Length; i++)
      {
        ShipsScriptable ss = shipList[i];
        _ships.Add(ss.shipName, ss);
      }
    }

    private void FindMaps()
    {
      for (int i = 0; i < mapList.Length; i++)
      {
        MapScriptable ms = mapList[i];
        _maps.Add(ms.name, ms);
      }
    }

    public void Back()
    {
      NetworkManager.Singleton.Shutdown();
      Destroy(NetworkManager.Singleton.gameObject);
      var go = SpawnManager.Instance.gameObject;
      SpawnManager.Instance.Nuke();
      SpawnManager.allowSpawning = false;
      var soundManagers = GameObject.FindObjectsOfType<SoundManager>();
      for (int i = 0; i < soundManagers.Length; i++) {
        Destroy(soundManagers[i].gameObject);
      }
      Destroy(GameObject.Find("/baws"));
      SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
      Debug.Log("HAJIME!");
      if (SpawnManager.Instance != null)
      {
        DFLogger.Instance.Log("We have a spawn manager");
        if (SpawnManager.Instance.GetClientId() != 0)
        {
          return;
        }

        bool canStart = true;
        for (int i = 0; i < SpawnManager.Instance._players.Count; i++)
        {
          MultiplayerMenuPlayer p = SpawnManager.Instance._players[i];
          if (!p.ready)
          {
            canStart = false;
          }
        }

        if (!canStart)
        {
          DFLogger.Instance.Log("we cannot start");
          return;
        }
      }

      var playerCount = NetworkManager.Singleton.ConnectedClients.Count;
      int.TryParse(lapSelector.value, out var lapCount);
      int.TryParse(botSelector.value, out var botCount);

      var racerList = new List<RacerInfo>();

      for (var i = 0; i < botCount; i++)
      {
        racerList.Add(new RacerInfo(_ships.Values.ToArray()));
      }

      for (int i = 0; i < SpawnManager.Instance._players.Count; i++) {
        var p = SpawnManager.Instance._players[i];
        racerList.Add(
          new RacerInfo(p.name.ToString(), shipList[p.shipIndex], (ulong)p.clientId)
        );
      }

      CrossScene.racerInfo = racerList.ToArray();
      CrossScene.map = mapList[mapSelector.index];
      CrossScene.laps = lapCount;
      CrossScene.players = playerCount;
      CrossScene.bots = botCount;
      CrossScene.cameFromMainMenu = true;
      SpawnManager.Instance.ResetReadies();
      Debug.Log($"MAPAPPPAPAPAPA: {CrossScene.map.name}");
      NetworkManager.Singleton.SceneManager.LoadScene(CrossScene.map.multiplayerScene, LoadSceneMode.Single);
    }
  }
}
