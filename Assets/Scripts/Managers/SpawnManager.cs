using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Utils;
using RelaySystem.Data;
using Unity.Collections;

namespace Managers
{
  public class SpawnManager : LonelyNetworkBehaviour<SpawnManager>
  {
    public event Action OnPlayerJoined;
    public event Action<int> ValueUpdate;
    public event Action<DFPlayer> OnClientJoined;
    public event Action<ulong> OnPlayerLeft;
    [HideInInspector] public NetworkList<MultiplayerMenuPlayer> _players = new NetworkList<MultiplayerMenuPlayer>();

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
        MultiplayerMenuPlayer p = new MultiplayerMenuPlayer(0);
        _players.Add(p);
      }
      _players.OnListChanged += OnSomeValueChanged;
    }

    private void Baws(ulong obj)
    {
      DFLogger.Instance.LogInfo("BAWS");
      Debug.Log("BAWS");
      int id = (int)NetworkManager.Singleton.LocalClientId;
      MultiplayerMenuPlayer p = new MultiplayerMenuPlayer(id);
      _players.Add(p);
    }

    private void OnSomeValueChanged(NetworkListEvent<MultiplayerMenuPlayer> evnt)
    {
      if (evnt.Type == NetworkListEvent<MultiplayerMenuPlayer>.EventType.Value)
      {
        ValueUpdate?.Invoke(evnt.Index);
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
      MultiplayerMenuPlayer old = _players[user_index];
      MultiplayerMenuPlayer newer = new MultiplayerMenuPlayer(old.name, old.clientId, ship_index, old.ready);
      _players[user_index] = newer;
    }

    public void SetPlayerReady(bool ready)
    {
      int userIndex = (int)NetworkManager.Singleton.LocalClientId;
      SetReadyServerRpc(userIndex, ready);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetReadyServerRpc(int user_index, bool ready)
    {
      MultiplayerMenuPlayer old = _players[user_index];
      MultiplayerMenuPlayer newer = new MultiplayerMenuPlayer(old.name, old.clientId, old.shipIndex, ready);
      _players[user_index] = newer;
    }

    public void SetPlayerName(string name)
    {
      int userIndex = (int)NetworkManager.Singleton.LocalClientId;
      SetNameServerRpc(userIndex, name);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetNameServerRpc(int user_index, string name)
    {
      MultiplayerMenuPlayer old = _players[user_index];
      MultiplayerMenuPlayer newer = new MultiplayerMenuPlayer((FixedString128Bytes)name, old.clientId, old.shipIndex, old.ready);
      _players[user_index] = newer;
    }

    [ServerRpc]
    private void DoSomeMagicServerRpc()
    {

    }
  }
}