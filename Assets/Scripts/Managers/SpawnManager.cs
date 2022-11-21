using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Utils;
using RelaySystem.Data;

namespace Managers
{
  public class SpawnManager : LonelyNetworkBehaviour<SpawnManager>
  {
    public event Action OnPlayerJoined;
    public event Action<int> ShipUpdate;
    public event Action<DFPlayer> OnClientJoined;
    public event Action<ulong> OnPlayerLeft;
    [HideInInspector] public NetworkList<bool> _playerReadies = new();
    [HideInInspector] public NetworkList<int> _playerShips = new();
    [HideInInspector] public NetworkList<int> _playerNames = new();

    public void Start()
    {
      Debug.Log("Spawn manager is managing some spawning");
      DFLogger.Instance.Log("Spawn manager is managing some spawning");

      NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
      NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
    }

    public override void OnNetworkSpawn()
    {
      if (IsServer)
      {
        NetworkManager.OnClientConnectedCallback += Baws;
        DFLogger.Instance.LogInfo("SPAWNINGING");
        _playerShips.Add(0);
      }
      _playerShips.OnListChanged += OnSomeValueChanged;
    }

    private void Baws(ulong obj)
    {
      DFLogger.Instance.LogInfo("BAWS");
      Debug.Log("BAWS");
      _playerShips.Add(0);
    }

    private void OnSomeValueChanged(NetworkListEvent<int> evnt)
    {
      if (evnt.Type == NetworkListEvent<int>.EventType.Value)
      {
        ShipUpdate.Invoke(evnt.Index);
      }
    }


    private void HostStarted()
    {
      DFLogger.Instance.Log("HOST STARTED");
    }

    private void ClientConnected(ulong clientId)
    {
    }

    private void ClientDisconnected(ulong clientId)
    {
      Debug.LogFormat($"{clientId} has left");
      if (IsServer)
      {
        OnPlayerLeft?.Invoke(clientId);
      }
    }

    public int CurrentPlayerIndex()
    {
      return (int)NetworkManager.Singleton.LocalClientId;
    }

    public void SetPlayerShip(int shipIndex)
    {
      int userIndex = (int)NetworkManager.Singleton.LocalClientId;
      SetShipServerRpc(userIndex, shipIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetShipServerRpc(int user_index, int ship_index)
    {
      _playerShips[user_index] = ship_index;
    }

    [ServerRpc]
    private void DoSomeMagicServerRpc()
    {

    }
  }
}