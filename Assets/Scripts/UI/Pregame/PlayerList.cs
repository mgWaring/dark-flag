using System.Collections.Generic;
using System.Linq;
using Managers;
using Multiplayer;
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
    float timer = 1.0f;
    bool sentName;
    //

    void Awake()
    {
      DFLogger.Instance.LogInfo("PLAYER LIST AWAKE");
    }

    void Update()
    {
      Debug.Log(HasNetworkObject);
      Debug.Log(_playerTiles.Count);
      Debug.Log(SpawnManager.Instance._players.Count);
      if (_playerTiles.Count < SpawnManager.Instance._players.Count)
      {
        CreateTile();
      }

      timer -= Time.deltaTime;
      if (timer <= 0 && !sentName)
      {
        SpawnManager.Instance.SetPlayerName(PlayerPrefs.GetString("playerName"));
        sentName = true;
      }
    }

    public void Start()
    {
      Debug.Log("Starting player list");
      DFLogger.Instance.Log("Starting player list");
      SpawnManager.Instance.OnPlayerJoined += CreateTile;
      SpawnManager.Instance.ValueUpdate += TileUpdate;
    }

    public bool AllReady()
    {
      return playerTileContainer.GetComponentsInChildren<ReadyButton>().All(
          tile => tile._ready
      );
    }

    private void TileUpdate(int i)
    {
      if (_playerTiles.Count <= i)
      {
        return;
      }

      MultiplayerMenuPlayer player = SpawnManager.Instance._players[i];

      ShipSelector ss = _playerTiles[i].GetComponentInChildren<ShipSelector>();
      ss.index = player.shipIndex;
      ss.DisplayValues();

      PlayerTile tile = _playerTiles[i].GetComponent<PlayerTile>();
      tile.UpdateName(player.name.ToString());
      tile.readyButton.SetReady(player.ready);
    }

    private void CreateTile()
    {
      DFLogger.Instance.LogInfo("CREATING TILE FOR CLIENT");
      var newTile = Instantiate(playerTilePrefab, playerTileContainer.transform, false);
      _playerTiles.Add(newTile);
    }
  }
}