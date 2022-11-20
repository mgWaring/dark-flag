using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Utils;
using RelaySystem.Data;

namespace Managers {
    public class SpawnManager : LonelyNetworkBehaviour<SpawnManager> {
        public event Action<DFPlayer> OnPlayerJoined;
        public event Action<DFPlayer> OnClientJoined;
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
                DFLogger.Instance.Log("Waiting 0.5 sec");
                yield return new WaitForSeconds(0.5f);
                DFLogger.Instance.Log("Done waiting, triggering self");
                WaitAndAddHost();
            }

            DFLogger.Instance.Log("Calling client connected in response to server starting");
            ClientConnected(NetworkManager.ServerClientId);
        }

        private IEnumerator WaitAndAddClient(DFPlayer player) {
            if (OnPlayerJoined == null) {
                DFLogger.Instance.Log("Waiting 0.5 sec for client");
                yield return new WaitForSeconds(0.5f);
                DFLogger.Instance.Log("client Done waiting, triggering self");
                WaitAndAddClient(player);
            }

            DFLogger.Instance.Log("Calling client connected in response to server starting");
            ClientConnected(NetworkManager.ServerClientId);
        }

        
        private IEnumerator WaitAndAddGenuineClient(DFPlayer player) {
            if (OnPlayerJoined == null) {
                DFLogger.Instance.Log("Waiting 0.5 sec for client");
                yield return new WaitForSeconds(0.5f);
                DFLogger.Instance.Log("client Done waiting, triggering self");
                WaitAndAddGenuineClient(player);
            }

            DFLogger.Instance.Log("Calling client connected in response to server starting");
            ClientConnected(NetworkManager.ServerClientId);
        }

        private void ClientConnected(ulong clientId) {
            if (IsClient) Debug.Log("I'm a client mama");
            if (IsHost) Debug.Log("I'm a host mama");
            if (IsServer) Debug.Log("I'm a server mama");
            Debug.Log($"{clientId} has connected");
            DFLogger.Instance.LogInfo($"{clientId} has connected");
            if (IsServer) {
                DFLogger.Instance.Log("I'm a server bebe");
                var DFPlayer = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<DFPlayer>();
                if(OnPlayerJoined != null) {
                    OnPlayerJoined.Invoke(DFPlayer);
                } else {
                    StartCoroutine(WaitAndAddClient(DFPlayer));
                }
            }
            if(IsClient){
                //work out how many already connected clients, and populate the tile list!
                var DFPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<DFPlayer>();
                if(OnClientJoined != null) {
                    OnClientJoined.Invoke(DFPlayer);
                } else {
                    StartCoroutine(WaitAndAddGenuineClient(DFPlayer));
                }
            }
        }

        private void ClientDisconnected(ulong clientId) {
            Debug.LogFormat($"{clientId} has left");
            if (IsServer) {
                OnPlayerLeft?.Invoke(clientId);
            }
        }

        [ServerRpc]
        private void DoSomeMagicServerRpc(){

        }
    }
}