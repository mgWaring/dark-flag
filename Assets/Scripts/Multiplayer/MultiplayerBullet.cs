using UnityEngine;
using Unity.Netcode;
using Managers;

namespace Multiplayer
{
  public class MultiplayerBullet : MonoBehaviour
  {
      public float damage;
      float maxLifetime = 10.0f;

      void Update()
      {
          maxLifetime -= Time.deltaTime;
          if (maxLifetime <= 0) {
            if (SpawnManager.Instance.GetClientId() == 0) {
              gameObject.GetComponent<NetworkObject>().Despawn();
              Destroy(gameObject);
            }
          }
      }

      private void OnTriggerEnter(Collider other) {
          if (other.gameObject.tag != "Checkpoint") {
            if (SpawnManager.Instance.GetClientId() == 0) {
              gameObject.GetComponent<NetworkObject>().Despawn();
              Destroy(gameObject);
            }
          }
      }
  }
}
