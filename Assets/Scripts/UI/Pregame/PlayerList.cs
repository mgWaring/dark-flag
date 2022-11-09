using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

namespace UI.Pregame {
    public class PlayerList : MonoBehaviour {
        private Dictionary<ulong, GameObject> _playerTiles = new();
        [SerializeField] private GameObject playerTilePrefab;
        [SerializeField] private GameObject playerTileContainer;

        public void Start() {
            SpawnManager.Instance.OnPlayerPrefsChange += UpdatePlayerList;
            UpdatePlayerList(SpawnManager.Instance.PlayerPrefs);
        }

        private void UpdatePlayerList(Dictionary<ulong, string> playerPrefs) {
            Debug.Log(playerPrefs.Count);
            if (playerPrefs.Count == 0) return;
            foreach (var pref in playerPrefs) {
                GameObject tile;
                if (_playerTiles.TryGetValue(pref.Key, out tile)) {
                    // update the existing tile
                    tile.GetComponent<PlayerTile>().pname = pref.Value;
                } else {
                    // if the prefs include an ID we don't have in our tile list, add a new tile
                    var newTile = Instantiate(playerTilePrefab, playerTileContainer.transform, false);
                    var newTileScript = newTile.GetComponent<PlayerTile>();
                    newTileScript.pid = pref.Key;
                    newTileScript.pname = pref.Value;
                    _playerTiles.Add(pref.Key, newTile);
                }

                //we also need to remove entries and tiles that aren't reflected by the pref data
                var tileIds = _playerTiles.Keys.ToArray();
                var prefIds = playerPrefs.Keys.ToArray();
                var leftovers = tileIds.Except(prefIds);
                foreach (var id in leftovers) {
                    var shinderuTile = _playerTiles[id];
                    _playerTiles.Remove(id);
                    Destroy(shinderuTile);
                }
            }
        }
    }
}