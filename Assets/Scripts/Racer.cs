using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer : MonoBehaviour
{
    [HideInInspector] public int id;
    [HideInInspector] public Checkpoint lastCheckpoint;
    [HideInInspector] public Checkpoint nextCheckpoint;

    public float GetPosition() {
        int b = lastCheckpoint.id;

        if (nextCheckpoint) {
            float total = Vector3.Distance(nextCheckpoint.transform.position, lastCheckpoint.transform.position);
            float current = Vector3.Distance(nextCheckpoint.transform.position, transform.position);
            float fraction = 1.0f - current/total;
            return (float)b + fraction;
        } else {
            return b;
        }
    }
}
