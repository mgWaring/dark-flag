using UnityEngine;
using UnityEngine.InputSystem;

//Add this script to vehicles that the player will be controlling.
public class PlayerMovement : MonoBehaviour
{
    private MovementController mc;
    private ShipDurability sd;
    public bool invertY;// doesn't do anything right now.
    public InputAction thrustInput;//Value is between -1 and 1.
    public InputAction yawInput;//Value is between -1 and 1.
    public InputAction cameraControl;
    public InputAction boostInput;//Value is either 0 or 1.
    public InputAction fireInput;//Value is either 0 or 1.
    public InputAction bombInput;//Value is either 0 or 1.
    public InputAction cameraFlip;//Value is either 0 or 1.

    private void Start()
    {
      mc = GetComponent<MovementController>();
      sd = GetComponent<ShipDurability>();
    }

    //Required for new input system. Don't ask me why.
    private void OnEnable()
    {
        thrustInput.Enable();
        yawInput.Enable();
        cameraControl.Enable();
        boostInput.Enable();
        fireInput.Enable();
        bombInput.Enable();
        cameraControl.Enable();
    }

    //Required for new input system. Don't ask me why.
    private void OnDisable()
    {
        thrustInput.Disable();
        yawInput.Disable();
        cameraControl.Disable();
        boostInput.Disable();
        fireInput.Disable();
        bombInput.Disable();
        cameraFlip.Disable();
    }

    private void Update()
    {
        mc.ThrustController(thrustInput.ReadValue<float>(), boostInput.ReadValue<float>());
        mc.YawController(yawInput.ReadValue<float>());
        sd.BoostDamage(boostInput.ReadValue<float>());
    }
}
