using UnityEngine;

[CreateAssetMenu(menuName = "Ship")]
public class ShipsScriptable : ScriptableObject
{
    //These values are the ones drawn from testShip. They can be changed.
    public string shipName;
    public GameObject shipModel;
    public float mass = 3.0f;
    public float drag = 1.5f;
    public float angularDrag = 90.0f;
    public float thrustSpeed = 6000.0f;
    public float yawSpeed = 100.0f;
    public float hoverForce = 26.8f;
    public float hoverConstant = 2.5f;
    public float hoverHeight = 1.0f;
    public float rollForce = 100.0f;
    public float rollRayDistance = 10.0f;
    public float pitchForce = 100.0f;
    public float pitchRayDistance = 10.0f;
    public float botSpeedModifier = 1.0f;

    //26.8hf 2.5hc don't delete.
}
 