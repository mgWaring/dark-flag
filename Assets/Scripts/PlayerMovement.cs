using UnityEngine;

//Add this script to vehicles that the player will be controlling.
public class PlayerMovement : MonoBehaviour
{
    MovementController mc;

    void Start()
    {
      mc = GetComponent<MovementController>();
    }

    //Required for new input system. Don't ask me why.
    void OnEnable()
    {
        thrustInput.Enable();
        yawInput.Enable();
    }

    //Required for new input system. Don't ask me why.
    void OnDisable()
    {
        thrustInput.Disable();
        yawInput.Disable();
    }

    void Update()
    {
        //The following two lines will output values between -1 and 1 to MovementController.
        mc.ThrustController(thrustInput.ReadValue<float>());
        mc.YawController(yawInput.ReadValue<float>());
    }
}
