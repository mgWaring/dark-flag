using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using RelaySystem.Data;
using UI.Pregame;
using Unity.Netcode;
using UnityEditor;

public class MultiPlayerOptions : MonoBehaviour {
    public Selector mapSelector;
    public Selector lapSelector;
    public Selector botSelector;
    private Dictionary<string, MapScriptable> _maps = new();
    private Dictionary<string, ShipsScriptable> _ships = new();
    [SerializeField] private PlayerList playerList;
    public ShipsScriptable[] shipList; 
    public MapScriptable[] mapList; 

    private void Start() {
        FindShips();
        FindMaps();
        if (false) {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    private void FindShips() {
        for (int i = 0; i < shipList.Length; i++) {
            ShipsScriptable ss = shipList[i];
            _ships.Add(ss.shipName, ss);
        }
    }

    private void FindMaps() {
        for (int i = 0; i < mapList.Length; i++) {
            MapScriptable ms = mapList[i];
            _maps.Add(ms.name, ms);
        }
    }

    public void Back() {
        Debug.Log("YOU MAY NEVER RETURN!!");
    }

    public void StartGame() {
        //could be more graceful with the ready check here
        if (!playerList.AllReady()) return;
        var playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        int.TryParse(lapSelector.value, out var lapCount);
        int.TryParse(botSelector.value, out var botCount);

        var racerList = new List<RacerInfo>();

        for (var i = 0; i < botCount; i++) {
            racerList.Add(new RacerInfo(_ships.Values.ToArray()));
        }

        foreach (var (_, client) in NetworkManager.Singleton.ConnectedClients) {
            var pdata = client.PlayerObject.GetComponent<DFPlayer>();
            racerList.Add(
                new RacerInfo(pdata.playerName, _ships[pdata.playerShipName], client.ClientId)
            );
        }

        CrossScene.racerInfo = racerList.ToArray();
        CrossScene.map = _maps[mapSelector.value];
        CrossScene.laps = lapCount;
        CrossScene.players = playerCount;
        CrossScene.bots = botCount;
        CrossScene.cameFromMainMenu = true;
        SceneManager.LoadScene(2);
    }
}