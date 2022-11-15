using System.Collections.Generic;
using System.Linq;
using Managers;
using RelaySystem.Data;
using UnityEngine;
using Unity.Netcode;

namespace UI.Pregame {
    public class PlayerList : NetworkBehaviour {
        private Dictionary<ulong, GameObject> _playerTiles = new();
        [SerializeField] private GameObject playerTilePrefab;
        [SerializeField] private GameObject playerTileContainer;

        public void Start() {
            if (IsServer) {
                CreateTileForClient(NetworkManager.LocalClient.PlayerObject);
            }
            SpawnManager.Instance.OnPlayerJoined += CreateTileForClient;
            SpawnManager.Instance.OnPlayerLeft += DeleteTileForClient;
        }

        private void CreateTileForClient(NetworkObject clientObject) {
            var DFPlayer = clientObject.GetComponent<DFPlayer>();
            var clientPlayerId = DFPlayer.playerID.Value;
            if (_playerTiles.TryGetValue(clientPlayerId, out var tile)) {
                // update the existing tile
                tile.GetComponent<PlayerTile>().Player = DFPlayer;
            } else {
                // if the prefs include an ID we don't have in our tile list, add a new tile
                var newTile = Instantiate(playerTilePrefab, playerTileContainer.transform, false);
                newTile.GetComponent<PlayerTile>().Player = DFPlayer;
                _playerTiles.Add(clientPlayerId, newTile);
            }

            //we also need to remove entries and tiles that aren't reflected by the pref data
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