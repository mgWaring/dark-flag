using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RelaySystem.Data;
using Unity.Netcode;
using Managers;

public class MultiPlayerOptions : MonoBehaviour
{
  public Selector mapSelector;
  public Selector lapSelector;
  public Selector botSelector;
  private Dictionary<string, MapScriptable> _maps = new();
  private Dictionary<string, ShipsScriptable> _ships = new();
  public ShipsScriptable[] shipList;
  public MapScriptable[] mapList;

  private void Start()
  {
    FindShips();
    FindMaps();
  }

  private void FindShips() {
    foreach (var ss in shipList) {
      _ships.Add(ss.shipName, ss);
    }
  }

  private void FindMaps() {
    foreach (var ms in mapList) {
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
      if (SpawnManager.Instance.GetClientId() != 0) return;
      
      var canStart = new List<bool>();
      foreach (var p in SpawnManager.Instance._players) canStart.Add(p.ready);
      
      //if any entry in the can start array is false, then we can't start
      if (canStart.Any(b => !b)) return;
      
    }

    var playerCount = NetworkManager.Singleton.ConnectedClients.Count;
    int.TryParse(lapSelector.value, out var lapCount);
    int.TryParse(botSelector.value, out var botCount);

    var racerList = new List<RacerInfo>();

    for (var i = 0; i < botCount; i++) racerList.Add(new RacerInfo(_ships.Values.ToArray()));
    
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

    //make a call to our friendly neighborhood network enabled scene management manager
    //(does that make it a  meta-manager? But then... who manages the managers that manage the managers?!)
    DFSceneManager.Instance.TryLoadScene("Multiplayer");
  }
}