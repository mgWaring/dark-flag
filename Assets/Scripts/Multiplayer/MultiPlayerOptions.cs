using System.Collections.Generic;
using System.Linq;
using Managers;
using RelaySystem.Data;
using UI.Pregame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer {
  public class MultiPlayerOptions : MonoBehaviour
  {
    public Selector mapSelector;
    public Selector lapSelector;
    public Selector botSelector;
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
      Debug.Log("YOU MAY NEVER RETURN!!");
    }

    public void StartGame()
    {
      if (SpawnManager.Instance != null)
      {
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

      foreach (var (_, client) in NetworkManager.Singleton.ConnectedClients)
      {
        var pdata = client.PlayerObject.GetComponent<DFPlayer>();
        var p = SpawnManager.Instance._players[(int)client.ClientId];
        racerList.Add(
          new RacerInfo(p.name.ToString(), shipList[p.shipIndex], client.ClientId)
        );
      }

      CrossScene.racerInfo = racerList.ToArray();
      CrossScene.map = _maps[mapSelector.value];
      CrossScene.laps = lapCount;
      CrossScene.players = playerCount;
      CrossScene.bots = botCount;
      CrossScene.cameFromMainMenu = true;
      NetworkManager.Singleton.SceneManager.LoadScene("Multiplayer", LoadSceneMode.Single);
    }
  }
}