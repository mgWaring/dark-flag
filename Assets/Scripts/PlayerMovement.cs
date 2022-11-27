using UnityEngine;
using UnityEngine.InputSystem;
using Multiplayer;
using UnityEngine.Windows;
using static UnityEditor.Rendering.CameraUI;

//Add this script to player.
public class PlayerMovement : MonoBehaviour
{
    MovementController mc;
    PlayerCameraController pcc;
    Turret trt;
    ShipDurability sd;
    FrontWeapon fw;
    BackWeapon bw;
    MultiplayerFrontWeapon mfw;
    MultiplayerBackWeapon mbw;
    public bool invertY = false;
    float yInversion;
    public InputAction thrustInput;//Value is between -1 and 1.
    public InputAction yawInput;//Value is between -1 and 1.
    public InputAction cameraControlY;//Value is between -1 and 1. Unity didn't like having x and y combined, so now they are seperate.
    public InputAction cameraControlX;//Value is between -1 and 1.
    public InputAction boostInput;//Value is either 0 or 1.
    public InputAction fireInput;//Value is either 0 or 1.
    public InputAction bombInput;//Value is either 0 or 1.
    public InputAction cameraFlip;//Value is either 0 or 1.

    void Start()
    {
        InverY();
        mc = GetComponentInChildren<MovementController>();
        pcc = GetComponentInChildren<PlayerCameraController>();
        trt = GetComponentInChildren<Turret>();
        sd = GetComponentInChildren<ShipDurability>();
        fw = GetComponentInChildren<FrontWeapon>();
        bw = GetComponentInChildren<BackWeapon>();              
        mfw = GetComponentInChildren<MultiplayerFrontWeapon>();
        mbw = GetComponentInChildren<MultiplayerBackWeapon>();              
    }

    //Required for new input system. Don't ask me why.
    void OnEnable()
    {
        thrustInput.Enable();
        yawInput.Enable();
        cameraControlY.Enable();
        cameraControlX.Enable();
        boostInput.Enable();
        fireInput.Enable();
        bombInput.Enable();
        cameraFlip.Enable();
    }

    //Required for new input system. Don't ask me why.
    void OnDisable()
    {
        thrustInput.Disable();
        yawInput.Disable();
        cameraControlY.Disable();
        cameraControlX.Disable();
        boostInput.Disable();
        fireInput.Disable();
        bombInput.Disable();
        cameraFlip.Disable();
    }

    //Calls relevent methods and gives them player input values.
    void Update()
    {
        mc.ThrustController(thrustInput.ReadValue<float>(), boostInput.ReadValue<float>());
        mc.YawController(yawInput.ReadValue<float>());
        sd.BoostDamage(boostInput.ReadValue<float>());
        pcc.CameraControl(cameraControlY.ReadValue<float>() * yInversion, cameraControlX.ReadValue<float>(), CameraFlipper(cameraFlip.ReadValue<float>()));
        trt.TurretControl(cameraControlX.ReadValue<float>(), CameraFlipper(cameraFlip.ReadValue<float>()));
        if (fw != null) {
          fw.ShootGun(fireInput.ReadValue<float>());
        }
        if (bw != null) {
          bw.BombRelease(bombInput.ReadValue<float>());
        }
        if (mfw != null) {
          mfw.ShootGun(fireInput.ReadValue<float>());
        }
        if (mbw != null) {
          mbw.BombRelease(bombInput.ReadValue<float>());
        }
    }

    //This one's for all you weirdos out there.
    void InverY()
    {
        if (invertY)
        {
            yInversion = -1;
        }
        else
        {
            yInversion = 1;
        }
    }

    //Turns cameraFlip into a usable value.
    float CameraFlipper(float input)
    {
        float output;
        if (input > 0)
        {
            output = -1;
        }
        else
        {
            output = 1;
        }
        return output;
    }


}
