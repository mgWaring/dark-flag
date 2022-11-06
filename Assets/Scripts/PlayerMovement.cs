using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    MovementController mc;
    public InputAction thrustInput;
    public InputAction yawInput;

    void Start()
    {
      mc = GetComponent<MovementController>();
    }

    private void OnEnable()
    {
        thrustInput.Enable();
        yawInput.Enable();
    }

    private void OnDisable()
    {
        thrustInput.Disable();
        yawInput.Disable();
    }

    void Update()
    {
        /*if (Input.GetKey(KeyCode.W)) {
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
        }*/

        mc.ThrustController(thrustInput.ReadValue<float>());
        mc.YawController(yawInput.ReadValue<float>());
    }
}
