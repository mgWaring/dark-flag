using System.Collections.Generic;
using System.Linq;
using Managers;
using RelaySystem.Data;
using UnityEngine;
using Unity.Netcode;
using Utils;

namespace UI.Pregame {
    public class PlayerList : MonoBehaviour {
        private Dictionary<ulong, GameObject> _playerTiles = new();
        [SerializeField] private GameObject playerTilePrefab;
        [SerializeField] private GameObject playerTileContainer;
        private NetworkList<int> _playerShips = new();
        public void Start() {
            Debug.Log("waking player list");
            DFLogger.Instance.Log("waking player list");
            SpawnManager.Instance.OnPlayerJoined += CreateTileForClient;
            SpawnManager.Instance.OnPlayerLeft += DeleteTileForClient;
            var count = NetworkManager.Singleton.ConnectedClients.Count;
            Debug.LogFormat($"there are {count} clients connected");
            CreateTileForClient(0);
        }

        public bool AllReady() {
            return playerTileContainer.GetComponentsInChildren<ReadyButton>().All(
                tile => tile._ready
            );
        }

        private void CreateTileForClient(ulong clientId) {
            var DFPlayer = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<DFPlayer>();
            var otherPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<DFPlayer>();
            Debug.Log(DFPlayer.playerName);
            Debug.LogFormat("We're creating a tile for a player with ID: {0}", DFPlayer.NetworkObject.OwnerClientId);
            if (_playerTiles.TryGetValue(clientId, out var tile)) {
                // update the existing tile
                tile.GetComponent<PlayerTile>().Player = DFPlayer;
            } else {
                // if the prefs include an ID we don't have in our tile list, add a new tile
                var newTile = Instantiate(playerTilePrefab, playerTileContainer.transform, false);
                newTile.GetComponent<PlayerTile>().Player = DFPlayer;
                _playerTiles.Add(clientId, newTile);
            }

            //we also need to remove entries and tiles that aren't reflected by the clients list
            var tileIds = _playerTiles.Keys.ToArray();
            var prefIds = NetworkManager.Singleton.ConnectedClients.Keys.ToArray();
            var leftovers = tileIds.Except(prefIds);
            foreach (var id in leftovers) {
                var shinderuTile = _playerTiles[id];
                _playerTiles.Remove(id);
                Destroy(shinderuTile);
            }
        }

        private void DeleteTileForClient(ulong clientId) {
            Destroy(_playerTiles[clientId]);
            _playerTiles.Remove(clientId);
        }
    }
}