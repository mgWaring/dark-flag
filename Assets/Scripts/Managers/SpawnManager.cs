using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace Managers {
    public class SpawnManager : LonelyNetworkBehaviour<SpawnManager> {
        private readonly NetworkVariable<int> _playersInGame = new();
        public Dictionary<ulong, string> PlayerPrefs { get; } = new();
        public event Action<Dictionary<ulong, string>> OnPlayerPrefsChange;
        public int PlayersInGame => _playersInGame.Value;

        public void Start() {
            NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
                if (IsServer)
                    Debug.LogFormat($"{id} has connected");
                PlayerPrefs.Add(id, RandomSensibleString.GenerateNameString());
                OnPlayerPrefsChange?.Invoke(PlayerPrefs);
                _playersInGame.Value++;
            };

            NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
                if (IsServer)
                    Debug.LogFormat($"{id} has left");
                PlayerPrefs.Remove(id);
                _playersInGame.Value--;
            };
        }
        
    }
}