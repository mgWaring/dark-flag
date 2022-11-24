using System;
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
    public event Action<int> MapUpdate;
    public event Action<int> LapUpdate;
    public event Action<int> BotUpdate;
    
    public NetworkList<MultiplayerMenuPlayer> _players = new();
    [HideInInspector] public NetworkVariable<int> _mapIndex = new();
    [HideInInspector] public NetworkVariable<int> _botIndex = new();
    [HideInInspector] public NetworkVariable<int> _lapIndex = new();
    [HideInInspector] public NetworkVariable<FixedString128Bytes> _code = new();

    public void Start()
    {
      Debug.Log("Spawn manager is managing some spawning");
      DFLogger.Instance.Log("Spawn manager is managing some spawning");
    }

    public override void OnNetworkSpawn()
    {
      if (IsServer)
      {
        NetworkManager.OnClientConnectedCallback += AddPlayerForClient;
        DFLogger.Instance.LogInfo("SPAWNINGING");
        MultiplayerMenuPlayer p = new MultiplayerMenuPlayer(0);
        _players.Add(p);
        _lapIndex.Value = 0;
        _mapIndex.Value = 0;
        _botIndex.Value = 0;
        _code.Value = RelayManager.Instance.CurrentRelayHostData.joinCode;
      }
      _players.OnListChanged += OnSomeValueChanged;
      _mapIndex.OnValueChanged += OnMapSelectionChanged;
      _lapIndex.OnValueChanged += OnLapSelectionChanged;
      _botIndex.OnValueChanged += OnBotSelectionChanged;
    }

    public int GetClientId()
    {
      return (int)NetworkManager.Singleton.LocalClientId;
    }

    private void AddPlayerForClient(ulong obj)
    {
      DFLogger.Instance.LogInfo("Adding MultiPlayer");
      Debug.Log("Adding MultiPlayer");
      int id = (int)NetworkManager.Singleton.LocalClientId;
      MultiplayerMenuPlayer p = new MultiplayerMenuPlayer(id);
      _players.Add(p);
    }

    private void OnMapSelectionChanged(int before, int after)
    {
      MapUpdate.Invoke(after);
    }

    private void OnLapSelectionChanged(int before, int after)
    {
      LapUpdate.Invoke(after);
    }

    private void OnBotSelectionChanged(int before, int after)
    {
      BotUpdate.Invoke(after);
    }

    private void OnSomeValueChanged(NetworkListEvent<MultiplayerMenuPlayer> evnt)
    {
      if (evnt.Type == NetworkListEvent<MultiplayerMenuPlayer>.EventType.Value)
      {
        ValueUpdate?.Invoke(evnt.Index);
      }
    }
    
    public int CurrentPlayerIndex()
    {
      return (int)NetworkManager.Singleton.LocalClientId;
    }

    public void SetSelection(string type, int index)
    {
      switch (type)
      {
        case "map":
          _mapIndex.Value = index;
          break;
        case "lap":
          _lapIndex.Value = index;
          break;
        case "bot":
          _botIndex.Value = index;
          break;
      }
    }

    public void SetPlayerShip(int shipIndex)
    {
      SetShipServerRpc(shipIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetShipServerRpc(int shipIndex, ServerRpcParams para = default)
    {
      int user_index = (int)para.Receive.SenderClientId;
      Debug.Log($"BEHING CALLED FOR {user_index} : {shipIndex}");
      MultiplayerMenuPlayer old = _players[user_index];
      MultiplayerMenuPlayer newer = new MultiplayerMenuPlayer(old.name, old.clientId, shipIndex, old.ready);
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
  }
}
