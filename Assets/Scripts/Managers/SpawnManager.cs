using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace Managers {
    public class SpawnManager : LonelyNetworkBehaviour<SpawnManager> {
        private readonly NetworkVariable<int> _playersInGame = new();
        public event Action<ulong> OnPlayerJoined;
        public event Action<ulong> OnPlayerLeft;

        public void Start() {
            Debug.Log("Spawn manager is managing some spawning");
            DFLogger.Instance.Log("Spawn manager is managing some spawning");

            NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
            NetworkManager.Singleton.OnServerStarted += () => { StartCoroutine(WaitAndAddHost()); };
        }

        private IEnumerator WaitAndAddHost() {
            if (OnPlayerJoined == null) {
                yield return new WaitForSeconds(0.5f);
                WaitAndAddHost();
            }

            ClientConnected(NetworkManager.ServerClientId);
        }

        private void ClientConnected(ulong clientId) {
            if (IsClient) Debug.Log("I'm a client mama");
            if (IsHost) Debug.Log("I'm a host mama");
            if (IsServer) Debug.Log("I'm a server mama");
            Debug.Log($"{clientId} has connected");
            DFLogger.Instance.LogInfo($"{clientId} has connected");
            if (IsServer) {
                OnPlayerJoined?.Invoke(clientId);
                _playersInGame.Value++;
            }
        }

        private void ClientDisconnected(ulong clientId) {
            Debug.LogFormat($"{clientId} has left");
            if (IsServer) {
                OnPlayerLeft?.Invoke(clientId);
                _playersInGame.Value--;
            }
        }
    }
}