using UnityEngine;

[CreateAssetMenu(menuName = "Ship")]
public class ShipsScriptable : ScriptableObject
{
    //Please take a note of all ship specific values (the ones in unity, not here) before messing around with this. Re ordering the list can reset ships to default values.
    public string shipName;
    public GameObject shipModel;
    [Header("Rigidbody")]
    public float mass = 3.0f;
    public float drag = 1.5f;
    public float angularDrag = 90.0f;
    [Header("MovementController")]
    public float thrustSpeed = 6000.0f;
    public float boost = 1.0f;    
    public float yawSpeed = 100.0f;
    public float torqueLimiter = 0.16f;
    public Quaternion turnTiltMaximum = Quaternion.Euler(0.0f, 0.0f, 38.0f);//Only the z is needed.
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
    [Header("Durability")]
    public float armour = 50.0f;
    public float fireDamage = 1.0f;
    public float boostDamageRate = 0.5f;
    public float roofRayDistance = 0.35f;
    public Vector3 roofRayOffset = new Vector3(0.0f, 0.0f, 0.0f);
    [Header("Other Values")]
    public float botSpeedModifier = 1.0f;
    public Vector3 cameraOffset = new Vector3(0.0f, 0.0f, 0.0f);
}
 