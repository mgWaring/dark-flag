using System;
using System.Threading.Tasks;
using RelaySystem.Data;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay;
using UnityEngine;
using Utils;

namespace Managers {
    public class RelayManager : LonelyMonoBehaviour<RelayManager> {
        [SerializeField] private int maxPlayers = 4;

        [SerializeField] private int maxConnections = 10;

        [SerializeField] private string environment = "production";

        public event Action<DFPlayer> OnStart;

        private UnityTransport Transport =>
            gameObject.GetComponent<UnityTransport>();

        private bool IsRelayEnabled =>
            Transport != null &&
            Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;

        private RelayHostData _currentRelayHostData;
        public RelayHostData CurrentRelayHostData => _currentRelayHostData;

        private Racer _thisPlayer;

        //observer pattern juju
        public delegate void ActiveRelayDelegate(RelayHostData relayHostData);

        public event ActiveRelayDelegate OnActiveRelayChange;

        public void Start() {
            Debug.Log("Lobby Manager bootstrapping, give it a sec");
            Debug.Log(IsRelayEnabled ? "relay is enabled" : "relay is not enabled");
        }

        public async Task<bool> HostGame() {
            var success = false;

            if (IsRelayEnabled) {
                await SetupRelay();
                success = NetworkManager.Singleton.StartHost();
            }

            Debug.Log(
                success
                    ? "Host started..."
                    : "Unable to start host..."
            );
            return success;
        }

        public async Task<bool> JoinGame(string joinCode) {
            var success = false;

            if (IsRelayEnabled && !string.IsNullOrEmpty(joinCode)) {
                await JoinRelay(joinCode);
                success = NetworkManager.Singleton.StartClient();
            }

            Debug.Log(
                success
                    ? "Client started..."
                    : "Unable to start client..."
            );
            return success;
        }

        private async Task<bool> SetupRelay() {
            Debug.Log("Setting up relay");
            try {
                var options = new InitializationOptions().SetEnvironmentName(environment);
                Debug.Log("Beginning Initialization");
                await UnityServices.InitializeAsync(options);
                Debug.Log("Finished Initialization");

                if (!AuthenticationService.Instance.IsSignedIn) {
                    Debug.Log("Looks like we need to sign in");
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                // todo -  the allocation here doesn't seem to complete...
                Debug.Log("Beginning Allocation");
                var allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);
                Debug.Log("Created Allocation");

                _currentRelayHostData = new RelayHostData {
                    key = allocation.Key,
                    port = (ushort)allocation.RelayServer.Port,
                    allocationID = allocation.AllocationId,
                    allocationIDBytes = allocation.AllocationIdBytes,
                    pv4Address = allocation.RelayServer.IpV4,
                    connectionData = allocation.ConnectionData
                };

                _currentRelayHostData.joinCode =
                    await Relay.Instance.GetJoinCodeAsync(_currentRelayHostData.allocationID);
                Debug.Log("Got join Code");

                Transport.SetRelayServerData(
                    _currentRelayHostData.pv4Address,
                    _currentRelayHostData.port,
                    _currentRelayHostData.allocationIDBytes,
                    _currentRelayHostData.key,
                    _currentRelayHostData.connectionData
                );

                Debug.Log($"Generated a lobby with the join code {_currentRelayHostData.joinCode}");
                OnActiveRelayChange?.Invoke(_currentRelayHostData);

                return true;
            } catch (Exception e) {
                Debug.LogFormat("Error trying to create a lobby: {0}", e);
            }

            return false;
        }

        private async Task<bool> JoinRelay(string joinCode) {
            Debug.LogFormat("Joining to relay server with {0}", joinCode);
            try {
                var options =
                    new InitializationOptions().SetEnvironmentName(environment);
                await UnityServices.InitializeAsync(options);
                if (!AuthenticationService.Instance.IsSignedIn) {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                var allocation =
                    await Relay.Instance.JoinAllocationAsync(joinCode);

                var relayJoinData =
                    new RelayJoinData {
                        key = allocation.Key,
                        port = (ushort)allocation.RelayServer.Port,
                        allocationID = allocation.AllocationId,
                        allocationIDBytes = allocation.AllocationIdBytes,
                        pv4Address = allocation.RelayServer.IpV4,
                        connectionData = allocation.ConnectionData,
                        hostConnectionData = allocation.HostConnectionData,
                        joinCode = joinCode
                    };

                Transport
                    .SetRelayServerData(
                        relayJoinData.pv4Address,
                        relayJoinData.port,
                        relayJoinData.allocationIDBytes,
                        relayJoinData.key,
                        relayJoinData.connectionData,
                        relayJoinData.hostConnectionData
                    );

                Debug.Log($"Client joined a lobby with the join code {relayJoinData.joinCode}");

                return true;
            } catch (Exception e) {
                Debug.LogFormat("Error trying to join a relay: {0}", e);
            }

            return false;
        }
    }
}