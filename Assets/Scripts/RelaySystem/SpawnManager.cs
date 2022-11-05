using Unity.Netcode;
using UnityEngine;
using Utils;

namespace RelaySystem {
    public class SpawnManager : LonelyNetworkBehaviour<SpawnManager> {
        private readonly NetworkVariable<int> _playersInGame = new NetworkVariable<int>(0);

        public int PlayersInGame => _playersInGame.Value;

        public void Start() {
            NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
                if (IsServer)
                    Debug.LogFormat($"{id} has connected");
                _playersInGame.Value++;
            };

            NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
                if (IsServer)
                    Debug.LogFormat($"{id} has left");
                _playersInGame.Value--;
            };
        }
    }
}