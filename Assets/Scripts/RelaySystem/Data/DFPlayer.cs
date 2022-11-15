using Unity.Netcode;
using UnityEngine;

/*
 *  * isClient & isOwner & isHost
 */
namespace RelaySystem.Data {
     public class DFPlayer : NetworkBehaviour {
    
          public NetworkString playerName;
          public NetworkVariable<ulong> playerID;
          public NetworkString playerShipName;
          //need to implement a serializable version of these two
          public Color playerColour;
          public AudioClip playerAnthem;
    
          // a new player enters the ring
          public override void OnNetworkSpawn() {
              // tell the spawn manager we exist
              
          }
          
     }
}