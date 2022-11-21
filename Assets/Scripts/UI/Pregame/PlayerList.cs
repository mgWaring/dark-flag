using System.Collections.Generic;
using System.Linq;
using Managers;
using RelaySystem.Data;
using UnityEngine;
using Unity.Netcode;
using Utils;

namespace UI.Pregame {
    public class PlayerList : NetworkBehaviour {
        private List<GameObject> _playerTiles = new();
        private NetworkList<bool> _playerReadies;
        private NetworkList<int> _playerShips;
        [SerializeField] private GameObject playerTilePrefab;
        [SerializeField] private GameObject playerTileContainer;
        public DFPlayer player;
        //

        void Awake() {
            DFLogger.Instance.LogInfo("PLAYER LIST AWAKE");
            _playerReadies = new NetworkList<bool>();
            _playerShips = new NetworkList<int>();
        }

        public void Start() {
            Debug.Log("waking player list");
            DFLogger.Instance.Log("waking player list");
            SpawnManager.Instance.OnPlayerJoined += CreateTileForClient;
            SpawnManager.Instance.OnPlayerLeft += DeleteTileForClient;            
            SpawnManager.Instance.OnClientJoined += DrawExistingTilesForClient;
        }

        public bool AllReady() {
            return playerTileContainer.GetComponentsInChildren<ReadyButton>().All(
                tile => tile._ready
            );
        }

        private void CreateTileForClient(DFPlayer DFPlayer) {
            DFLogger.Instance.LogInfo("CREATING TILE FOR CLIENT");
            var clientId = System.Convert.ToInt32(DFPlayer.OwnerClientId);
            Debug.Log(DFPlayer.playerName);
            Debug.LogFormat("We're creating a tile for a player with ID: {0}", DFPlayer.NetworkObject.OwnerClientId);
            if (_playerTiles.Count < clientId) {
                // update the existing tile
                _playerTiles[clientId].GetComponent<PlayerTile>().Player = DFPlayer;
            } else {
                // if the prefs include an ID we don't have in our tile list, add a new tile
                var newTile = Instantiate(playerTilePrefab, playerTileContainer.transform, false);
                newTile.GetComponent<PlayerTile>().Player = DFPlayer;
                _playerTiles.Add(newTile);
                _playerReadies.Add(false);
                _playerShips.Add(0);
            }

        }

        private void DeleteTileForClient(ulong beans) {
            var clientId = System.Convert.ToInt32(beans);
            Destroy(_playerTiles[clientId]);
            _playerTiles.RemoveAt(clientId);
        }

        public void DrawExistingTilesForClient(DFPlayer player){
            DFLogger.Instance.Log("I would add another tile");
            for(var i = 0; i < _playerReadies.Count; i++) {
                DFLogger.Instance.Log($"I would add another tile for player {i}");
            }
        }
    }
}