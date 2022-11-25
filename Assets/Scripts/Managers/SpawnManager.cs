using System;
using Unity.Netcode;
using UnityEngine;
using Utils;
using RelaySystem.Data;
using Unity.Collections;
using Multiplayer;
using UnityEngine.SceneManagement;

namespace Managers
{
  public class SpawnManager : LonelyNetworkBehaviour<SpawnManager>
  {
    public event Action OnPlayerJoined;
    public event Action<int> OnPlayerLeave;
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
      NetworkManager.OnClientDisconnectCallback += RemovePlayerForClient;
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

    public void ResetReadies() {
      for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++) {
        MultiplayerMenuPlayer old = _players[i];
        MultiplayerMenuPlayer newer = new MultiplayerMenuPlayer(old.name, old.clientId, old.shipIndex, false);
        _players[i] = newer;
      }
    }

    public int GetClientId()
    {
      return (int)NetworkManager.Singleton.LocalClientId;
    }

    private void AddPlayerForClient(ulong obj)
    {
      DFLogger.Instance.LogInfo("Adding MultiPlayer");
      Debug.Log("Adding MultiPlayer");
      int id = (int)obj;
      MultiplayerMenuPlayer p = new MultiplayerMenuPlayer(id);
      _players.Add(p);
    }

    private void RemovePlayerForClient(ulong obj)
    {
      if (obj == 0) {
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        var go = SpawnManager.Instance.gameObject;
        SpawnManager.Instance.Nuke();
        SpawnManager.allowSpawning = false;
        var soundManagers = GameObject.FindObjectsOfType<SoundManager>();
        for (int i = 0; i < soundManagers.Length; i++) {
          Destroy(soundManagers[i].gameObject);
        }
        Destroy(GameObject.Find("/baws"));
        SceneManager.LoadScene(0);
      }

      if (GetClientId() == 0) {
        int index = 0;
        for (int i = 0; i < _players.Count; i++) {
          if (_players[i].clientId == (int)obj) {
            index = i;
          }
        }
        _players.RemoveAt(index);
      }
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

    public void Nuke() {
      Destroy(gameObject);
      Destroy(this);
    }

    private void OnSomeValueChanged(NetworkListEvent<MultiplayerMenuPlayer> evnt)
    {
      if (evnt.Type == NetworkListEvent<MultiplayerMenuPlayer>.EventType.Value)
      {
        ValueUpdate?.Invoke(evnt.Index);
      } else if (evnt.Type == NetworkListEvent<MultiplayerMenuPlayer>.EventType.RemoveAt) {
        OnPlayerLeave?.Invoke(evnt.Index);
      }
    }
    
    public int CurrentPlayerIndex()
    {
      return (int)NetworkManager.Singleton.LocalClientId;
    }

    public int IndexFor(ulong id)
    {
      int index = 0;
      for (int i = 0; i < _players.Count; i++) {
        if (_players[i].clientId == (int)id) {
          index = i;
        }
      }
      return index;
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
      ulong id = para.Receive.SenderClientId;
      int user_index = IndexFor(id);
      MultiplayerMenuPlayer old = _players[user_index];
      MultiplayerMenuPlayer newer = new MultiplayerMenuPlayer(old.name, old.clientId, shipIndex, old.ready);
      _players[user_index] = newer;
    }

    public void SetPlayerReady(bool ready)
    {
      SetReadyServerRpc(ready);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetReadyServerRpc(bool ready, ServerRpcParams para = default)
    {
      ulong id = para.Receive.SenderClientId;
      int user_index = IndexFor(id);
      MultiplayerMenuPlayer old = _players[user_index];
      MultiplayerMenuPlayer newer = new MultiplayerMenuPlayer(old.name, old.clientId, old.shipIndex, ready);
      _players[user_index] = newer;
    }

    public void SetPlayerName(string name)
    {
      SetNameServerRpc(name);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetNameServerRpc(string name, ServerRpcParams para = default)
    {
      ulong id = para.Receive.SenderClientId;
      int user_index = IndexFor(id);
      MultiplayerMenuPlayer old = _players[user_index];
      MultiplayerMenuPlayer newer = new MultiplayerMenuPlayer((FixedString128Bytes)name, old.clientId, old.shipIndex, old.ready);
      _players[user_index] = newer;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnBombServerRpc(ServerRpcParams para = default)
    {
      ulong id = para.Receive.SenderClientId;
      var po = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;
      po.GetComponentInChildren<MultiplayerBackWeapon>().SpawnBomb();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnBulletServerRpc(ServerRpcParams para = default)
    {
      ulong id = para.Receive.SenderClientId;
      var po = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;
      po.GetComponentInChildren<MultiplayerFrontWeapon>().SpawnBullet();
    }
  }
}
