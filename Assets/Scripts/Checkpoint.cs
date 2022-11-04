using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [HideInInspector] public int id;
    public GameController gameController;

    void Start() {
        id = transform.GetSiblingIndex();
    }

    void OnTriggerEnter(Collider collider) {
        if(collider.tag == "Ship") {
            Racer racer = collider.gameObject.GetComponentInParent<Racer>();

            if (this == racer.nextCheckpoint) {
                Checkpoint next = NextCheckpoint();
                if (next == null) {
                    gameController.registerFinish(racer);
                } else {
                    racer.lastCheckpoint = this;
                    racer.nextCheckpoint = next;
                }
            }
        }
    }

    Checkpoint NextCheckpoint() {
        int max = transform.parent.childCount - 1;
        if (id + 1 > max) {
            return null;
        }
        Transform obj = transform.parent.GetChild(id + 1);
        return obj.GetComponent<Checkpoint>();
    }
}
