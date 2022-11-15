using UnityEngine;

[CreateAssetMenu(menuName = "Ship")]
public class ShipsScriptable : ScriptableObject
{
    //These values are the ones drawn from testShip. They can be changed.
    public string shipName;
    public GameObject shipModel;
    [Header("Rigidbody")]
    public float mass = 3.0f;
    public float drag = 1.5f;
    public float angularDrag = 90.0f;
    [Header("MovementController")]
    public float thrustSpeed = 6000.0f;
    public float yawSpeed = 100.0f;
    [Header("AntiGravManager")]
    public float hoverForce = 26.8f;
    public float hoverStiffness = 2.5f;
    public float pitchRollStiffness = 4.3f;
    public float rollForce = 100.0f;
    public float rollRayDistance = 10.0f;
    public float pitchForce = 100.0f;
    public float pitchRayDistance = 10.0f;    
    public Vector3 colliderCalcOffset = new Vector3(0.15f, 0.0f, 0.0f);
    public Vector3 fineTuneOffset = new Vector3(0.0f, 0.0f, 0.0f);
    [Header("Other Values")]
    public float botSpeedModifier = 1.0f;
    public float armour = 0.0f;

    //26.8hf 2.5hc don't delete.
}
 