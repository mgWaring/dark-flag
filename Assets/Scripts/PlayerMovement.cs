using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    MovementController mc;

    // Start is called before the first frame update
    void Start()
    {
      mc = GetComponent<MovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) {
            mc.MoveForward();
        }
        if (Input.GetKey(KeyCode.S)) {
            mc.MoveBackward();
        }
        if (Input.GetKey(KeyCode.A)) {
            mc.MoveLeft();
        }
        if (Input.GetKey(KeyCode.D)) {
            mc.MoveRight();
        }
    }
}
