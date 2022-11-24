using System.Collections.Generic;
using System.Linq;
using Managers;
using Multiplayer;
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
    float timer = 0.5f;
    bool firstUpdateDone;
    //

    void Awake()
    {
      DFLogger.Instance.LogInfo("PLAYER LIST AWAKE");
    }

    void Update()
    {
      timer -= Time.deltaTime;
      if (timer <= 0 && !firstUpdateDone)
      {
        SpawnManager.Instance.SetPlayerName(PlayerPrefs.GetString("playerName"));
        firstUpdateDone = true;

        for (int i = 0; i < _playerTiles.Count; i++) {
          TileUpdate(i);
        }
      }

      if (_playerTiles.Count < SpawnManager.Instance._players.Count) {
        int diff = SpawnManager.Instance._players.Count - _playerTiles.Count;
        for (int i = 0; i < diff; i++) {
          CreateTile();
        }
      }
    }

    public void Start()
    {
      SpawnManager.Instance.OnPlayerJoined += CreateTile;
      SpawnManager.Instance.ValueUpdate += TileUpdate;
      if (SpawnManager.Instance.GetClientId() == 0) {
        CreateTile();
      }
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

      MultiplayerShipSelector ss = _playerTiles[i].GetComponentInChildren<MultiplayerShipSelector>();
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
