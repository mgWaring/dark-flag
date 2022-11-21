using Unity.Netcode;
using UnityEngine;

/*
 *  * isClient & isOwner & isHost
 */
namespace RelaySystem.Data {
     public class DFPlayer : NetworkBehaviour {
          public NetworkVariable<int> _selectedShipIndex = new();
          public NetworkVariable<bool> _ready = new();

          public bool shipChanged = false;
          public string playerName;
          public string playerShipName;
          //need to implement a serializable version of these two
          public Color playerColour;
          public AudioClip playerAnthem;

          public GameObject Ship { get; set; }
          // a new player enters the ring
          public override void OnNetworkSpawn() {
              // tell the spawn manager we exist
              Debug.LogFormat("I HAVE AWOKEN, and my id is: {0}", OwnerClientId);
              if (IsClient) Debug.Log("I'm a client DFPLAYER");
              if (IsHost) Debug.Log("I'm a host DFPLAYER");
              if (IsServer) Debug.Log("I'm a server DFPLAYER");
              if (IsClient && IsOwner) Debug.Log("I'm your local DFPlayer bebe");
              DontDestroyOnLoad(gameObject);
              _selectedShipIndex.Value = 0;
              _ready.Value = false;
              _selectedShipIndex.OnValueChanged += UpdateShipSelector;
          }

          private void UpdateShipSelector(int previous, int current) {
            shipChanged = true;
          }
        

          //just in case we want to do something to the ship
          public void ClaimShip(GameObject ship) {
              Ship = ship;
          }

          void Start() {
            playerName = PlayerPrefs.GetString("playerName");
          }
     }
}