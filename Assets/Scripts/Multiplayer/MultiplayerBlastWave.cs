using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Unity.Netcode;

namespace Multiplayer {
  public class MultiplayerBlastWave : MonoBehaviour
  {
      public int points;
      public float maxRadius;
      public float speed;
      public float maxWidth;
      public float delay;
      public float force;
      public float damage;
      LineRenderer lineRenderer;
      AudioSource audioSource;
      float timer;
      bool exploded = false;
      List<GameObject> damagedObjects = new List<GameObject>();

      void Start() {
          audioSource = GetComponent<AudioSource>();
          lineRenderer = GetComponent<LineRenderer>();
          lineRenderer.positionCount = points + 1;
          timer = delay;
      }

      void Update() {
          timer -= Time.deltaTime;
          if (timer <= 0.0f && !exploded) {
              exploded = true;
              audioSource.Play();
              StartCoroutine(Blast());
          }
      } 

      IEnumerator Blast() {
          float currentRadius = 0f;
          while (currentRadius <= maxRadius) {
              currentRadius += speed * Time.deltaTime;
              Draw(currentRadius);
              Damage(currentRadius);
              yield return null;
          }
      }

      void Draw(float currentRadius) {
          float angleBetweenPoints = 360.0f / points;

          for (int i = 0; i <= points; i ++) {
              float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
              Vector3 direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f);
              Vector3 position = direction * currentRadius;

              lineRenderer.SetPosition(i, position);
          }

          lineRenderer.widthMultiplier = Mathf.Lerp(0f, maxWidth, 1f - currentRadius / maxRadius);
          if (currentRadius >= maxRadius) {
              if (SpawnManager.Instance.GetClientId() == 0) {
                gameObject.GetComponent<NetworkObject>().Despawn();
                Destroy(gameObject);
              }
          }
      }

      void OnTriggerEnter(Collider other) {
          if (other.gameObject.tag != "Checkpoint") {
              exploded = true;
              audioSource.Play();
              StartCoroutine(Blast());
          }
      }

      void Damage(float currentRadius) {
          Collider[] objectsHit = Physics.OverlapSphere(transform.position, currentRadius);

          for (int i = 0; i < objectsHit.Length; i++) {
              Rigidbody rb = objectsHit[i].GetComponent<Rigidbody>();
              if (!rb) {
                  continue;
              }
              ShipDurability durability = objectsHit[i].GetComponent<ShipDurability>();

              GameObject go = objectsHit[i].gameObject;
              if (durability != null && !damagedObjects.Contains(go)) {
                  durability.takeDamage(damage);
                  damagedObjects.Add(go);
              }

              Vector3 direction = (objectsHit[i].transform.position - transform.position).normalized;
              rb.AddForce(direction * force, ForceMode.Impulse);
          }
      }
  }
}
