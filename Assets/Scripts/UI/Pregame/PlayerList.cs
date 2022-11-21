using System.Collections.Generic;
using System.Linq;
using Managers;
using RelaySystem.Data;
using UnityEngine;
using Unity.Netcode;
using Utils;

namespace UI.Pregame
{
  public class PlayerList : NetworkBehaviour
  {
    private List<GameObject> _playerTiles = new();
    [SerializeField] private GameObject playerTilePrefab;
    [SerializeField] private GameObject playerTileContainer;
    float timer = 10.0f;
    //

    void Awake()
    {
      DFLogger.Instance.LogInfo("PLAYER LIST AWAKE");
    }

    public override void OnNetworkSpawn()
    {
      DFLogger.Instance.LogInfo("wdeadwad");
      SpawnManager.Instance._playerReadies.Add(false);
      SpawnManager.Instance._playerShips.Add(1);
      SpawnManager.Instance._playerNames.Add(100);
    }

    void Update()
    {
      Debug.Log(HasNetworkObject);
      Debug.Log(_playerTiles.Count);
      Debug.Log(SpawnManager.Instance._playerShips.Count);
      if (_playerTiles.Count < SpawnManager.Instance._playerShips.Count)
      {
        CreateTile();
      }
    }

    public void Start()
    {
      Debug.Log("Starting player list");
      DFLogger.Instance.Log("Starting player list");
      SpawnManager.Instance.OnPlayerJoined += CreateTile;
      SpawnManager.Instance.OnPlayerLeft += DeleteTileForClient;
      SpawnManager.Instance.ShipUpdate += ShipUpdate;
    }

    public bool AllReady()
    {
      return playerTileContainer.GetComponentsInChildren<ReadyButton>().All(
          tile => tile._ready
      );
    }

    private void ShipUpdate(int i)
    {
      if (_playerTiles.Count <= i)
      {
        return;
      }

      ShipSelector ss = _playerTiles[i].GetComponentInChildren<ShipSelector>();
      ss.index = SpawnManager.Instance._playerShips[i];
      ss.DisplayValues();
    }

    private void CreateTile()
    {
      Utils.DFLogger.Instance.LogInfo("CREATING TILE FOR CLIENT");
      var newTile = Instantiate(playerTilePrefab, playerTileContainer.transform, false);
      _playerTiles.Add(newTile);
    }

    private void DeleteTileForClient(ulong beans)
    {
      //var clientId = System.Convert.ToInt32(beans);
      //Destroy(_playerTiles[clientId]);
      //_playerTiles.RemoveAt(clientId);
    }

    public void DrawExistingTilesForClient()
    {
      //DFLogger.Instance.Log("I would add another tile");
      //for(var i = 0; i < SpawnManager.Instance._playerReadies.Count; i++) {
      //    DFLogger.Instance.Log($"I would add another tile for player {i}");
      //}
    }
  }
}