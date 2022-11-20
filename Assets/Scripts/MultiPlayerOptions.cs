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
    private Dictionary<string, MapScriptable> _maps;
    private Dictionary<string, ShipsScriptable> _ships = new();
    [SerializeField] private PlayerList playerList;

    private void Start() {
        FindShips();
        FindMaps();
    }

    private void FindShips() {
        var theShipIds = AssetDatabase.FindAssets("t:ShipsScriptable", new[] { "Assets/ScriptableObjects/Ships" });

        foreach (var guid in theShipIds) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var theShip = (ShipsScriptable)AssetDatabase.LoadAssetAtPath(path, typeof(ShipsScriptable));
            _ships.Add(theShip.shipName, theShip);
        }
    }

    private void FindMaps() {
        var mapIds = AssetDatabase.FindAssets("t:MapScriptable", new[] { "Assets/MapScriptableObjects/Maps" });

        foreach (var guid in mapIds) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var theMap = (MapScriptable)AssetDatabase.LoadAssetAtPath(path, typeof(MapScriptable));
            _maps.Add(theMap.name, theMap);
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