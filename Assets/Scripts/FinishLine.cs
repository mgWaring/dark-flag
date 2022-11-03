using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public GameController gameController;


    void OnTriggerEnter(Collider collider) {
        if(collider.tag == "Ship") {
            Racer racer = collider.gameObject.GetComponent<Racer>();
            gameController.registerFinish(racer);
        }
    }
}
