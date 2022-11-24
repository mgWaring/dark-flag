using UnityEngine;
using Managers;
using Unity.Netcode;

namespace Multiplayer
{
  public class MultiplayerBackWeapon : MonoBehaviour
  {
      public GameObject bombPrefab;
      public Transform bombSpawn;
      public int ammoCount;
      public float firingDelay;
      float currentFiringDelay;

      void Start()
      {
          currentFiringDelay = firingDelay;
      }

      void OnTriggerEnter(Collider other) {
          if (other.tag == "Pickup") {
              Pickup pickup = other.gameObject.GetComponent<Pickup>();
              if (pickup.type == Pickup.PickupType.Ammo) {
                  ammoCount++;
                  Destroy(other.gameObject);
              }
          }
      }

      public void BombRelease(float input)
      {
          currentFiringDelay -= Time.deltaTime;
          if (input > 0 && currentFiringDelay <= 0 && ammoCount > 0)
          {
              ammoCount--;
              currentFiringDelay = firingDelay;
              SpawnManager.Instance.SpawnBombServerRpc();
          }
      }

      public void SpawnBomb()
      {
          GameObject bomb = Instantiate(bombPrefab);
          bomb.transform.position = bombSpawn.position;
          bomb.transform.rotation = bombSpawn.rotation;
          bomb.transform.Rotate(90f, 0f, 0f);
          bomb.GetComponent<Rigidbody>().AddForce(bomb.transform.up * 10f, ForceMode.Impulse);
          bomb.GetComponent<NetworkObject>().Spawn();
      }
  }
}
