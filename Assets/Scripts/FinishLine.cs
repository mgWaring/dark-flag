using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public GameController gameController;


    void OnTriggerEnter(Collider collider) {
        // Get Name from ship
        if(collider.tag == "Ship") {
            gameController.registerFinish(string.Format("{0}", Random.Range(0.0f, 100000.0f)));
        }
    }
}
