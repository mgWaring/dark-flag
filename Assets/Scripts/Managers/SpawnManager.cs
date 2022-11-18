using System;
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

        public event Action<ulong> OnPlayerJoined;
        public event Action<ulong> OnPlayerLeft;
        public int PlayersInGame => _playersInGame.Value;

        public void Start() {
            Debug.Log("Spawn manager is managing some spawning");

            NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
        }

        private void ClientConnected(ulong clientId) {
            if (IsClient) Debug.Log("I'm a client mama");
            if (IsHost) Debug.Log("I'm a host mama");
            if (IsServer) Debug.Log("I'm a server mama");
            Debug.LogFormat($"{clientId} has connected");
            OnPlayerJoined?.Invoke(clientId);
            _playersInGame.Value++;
        }

        private void ClientDisconnected(ulong clientId) {
            Debug.LogFormat($"{clientId} has left");
            OnPlayerLeft?.Invoke(clientId);
            _playersInGame.Value--;
        }
    }
}