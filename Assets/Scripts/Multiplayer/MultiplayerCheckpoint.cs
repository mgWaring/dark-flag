using System.Linq;
using UnityEngine;

namespace Multiplayer {
    public class MultiplayerCheckpoint : MonoBehaviour
    {
        [HideInInspector] public int id;
        [HideInInspector] public MultiplayerGameController gameController;
        Transform[] spawnPoints;

        void Start() {
            spawnPoints = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++) {
                spawnPoints[i] = transform.GetChild(i);
            }
            id = transform.GetSiblingIndex();
            gameController = GameObject.Find("/GameController").GetComponent<MultiplayerGameController>();
        }

        public Transform spawnPointFor(Vector3 pos) {
            if (spawnPoints.Length == 0) {
                return transform;
            }
            if (spawnPoints.Length == 1) {
                return spawnPoints[0];
            } 
            return spawnPoints.OrderBy(t => Mathf.Abs(Vector3.Distance(pos, t.position))).ToArray()[0];
        }

        void OnTriggerEnter(Collider collider) {
            if(collider.tag == "Ship") {
                MultiplayerRacer racer = collider.gameObject.GetComponentInParent<MultiplayerRacer>();

                if (this == racer.nextCheckpoint) {
                    MultiplayerCheckpoint next = NextCheckpoint();
                    if (next == null) {
                        racer.lastCheckpoint = this;
                        racer.nextCheckpoint = transform.parent.GetChild(0).GetComponent<MultiplayerCheckpoint>();
                    } else {
                        racer.lastCheckpoint = this;
                        racer.nextCheckpoint = next;
                        if (gameController != null && id == 0) {
                            gameController.RegisterLap(racer);
                        }
                    }
                }
            }
        }

        MultiplayerCheckpoint NextCheckpoint() {
            int max = transform.parent.childCount - 1;
            if (id + 1 > max) {
                return null;
            }
            Transform obj = transform.parent.GetChild(id + 1);
            return obj.GetComponent<MultiplayerCheckpoint>();
        }
    }
}
