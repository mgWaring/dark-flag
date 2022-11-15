using System;
using System.Collections.Generic;
using RelaySystem.Data;
using Unity.Netcode;
using UnityEngine;
using Utils;

/**
 * todo:
 * Extend this so that it can handle spawning the correct ship for each player when we load a level
 *
 */


namespace Managers {
    public class SpawnManager : LonelyNetworkBehaviour<SpawnManager> {
        private readonly NetworkVariable<int> _playersInGame = new();
        //this is a list of GameObjects that represents the players connected to our game
        
        public event Action<NetworkObject> OnPlayerJoined;
        public event Action<ulong> OnPlayerLeft;
        public int PlayersInGame => _playersInGame.Value;
        public NetworkObject PlayerObject;

        public void Start() {
            NetworkManager.Singleton.OnClientConnectedCallback += (clientId) => {
                if (IsServer) {
                    Debug.LogFormat($"{clientId} has connected");

                    PlayerObject.SpawnAsPlayerObject(clientId);
                    OnPlayerJoined?.Invoke(
                            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject
                        );
                    _playersInGame.Value++;
                }
            };

            NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
                if (IsServer)
                    Debug.LogFormat($"{id} has left");
                OnPlayerLeft?.Invoke(id);
                _playersInGame.Value--;
            };
        }
        
    }
}