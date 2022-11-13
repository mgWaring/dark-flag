using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [HideInInspector] public int id;
    GameController gameController;

    void Start() {
        id = transform.GetSiblingIndex();
        gameController = GameObject.Find("/GameController").GetComponent<GameController>();
    }

    void OnTriggerEnter(Collider collider) {
        if(collider.tag == "Ship") {
            Racer racer = collider.gameObject.GetComponentInParent<Racer>();

            if (this == racer.nextCheckpoint) {
                Checkpoint next = NextCheckpoint();
                if (next == null) {
                    racer.lastCheckpoint = this;
                    racer.nextCheckpoint = transform.parent.GetChild(0).GetComponent<Checkpoint>();
                } else {
                    racer.lastCheckpoint = this;
                    racer.nextCheckpoint = next;
                    if (gameController != null && id == 0) {
                        gameController.registerLap(racer);
                    }
                    RacerAgent agent = racer.gameObject.GetComponent<RacerAgent>();
                    if (agent != null) {
                        agent.AddCheckpoint(id);
                    }
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
