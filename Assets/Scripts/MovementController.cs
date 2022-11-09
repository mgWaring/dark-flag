using UnityEngine;

//Add this script to the vehicle object that has a rigidbody.
public class MovementController : MonoBehaviour
{    
    //Change accelerationVar to adjust forward and backwards speed.
    float accelerationVar;
    //Change yawSpeedVar to adjust yaw rotational speed.
    float yawSpeedVar;
    //accelerationMult and yawMult determine direction as positive or negative numbers.
    float accelerationMult = 0;
    float yawSpeedMult = 0;    
    float acceleration = 0;
    float yawSpeed = 0;
    Vector3 yawAngularVelocity = new Vector3(0, 0, 0);    

    Vector3 oldPosition;
    Vector3 newPosition;
    Vector3 distanceTravelledMath;
    float newTimeStamp;
    float oldTimeStamp;
    float timePassed;
    float distanceTravelled = 0;
    float vehicleVelocity;

    Rigidbody vehicleRB;
    [HideInInspector] public ShipsScriptable ss;

    public void Reset() {
        accelerationMult = 0;
        yawSpeedMult = 0;
        acceleration = 0;
        yawSpeed = 0;
        yawAngularVelocity = Vector3.zero;
        vehicleRB.velocity = Vector3.zero;
        vehicleRB.angularVelocity = Vector3.zero;
    }

    private void Start()
    {
        vehicleRB = GetComponent<Rigidbody>();
        accelerationVar = ss.thrustSpeed;
        yawSpeedVar = ss.yawSpeed;
    }

    void FixedUpdate()
    {        
        //Tracks speed.
        newPosition = transform.position;
        newTimeStamp = Time.realtimeSinceStartup;
        vehicleVelocity = VelocityCalculator(newPosition, oldPosition,  newTimeStamp, oldTimeStamp);

        //Adds thrust force to vehicle.
        acceleration = SpeedSet(accelerationMult, accelerationVar);
        vehicleRB.AddRelativeForce(Vector3.forward * acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);        

        //Rotates vehicle.
        yawSpeed = SpeedSet(yawSpeedMult, yawSpeedVar);
        yawAngularVelocity = Vector3.up * yawSpeed;
        Quaternion yawDeltaRotation = Quaternion.Euler(yawAngularVelocity * Time.fixedDeltaTime);
        vehicleRB.MoveRotation(vehicleRB.rotation * yawDeltaRotation);

        //Part of speed tracking.
        oldPosition = transform.position;
        oldTimeStamp = Time.realtimeSinceStartup;
    }

    public void ThrustController(float input)
    {
        accelerationMult = input;
    }

    public void YawController(float input)
    {
        yawSpeedMult = input;
    }

    float SpeedSet(float multiplier, float variable)
    {
        float speed;
        speed = multiplier * variable;

        return speed;
    }

    //VelocityCalculator is no longer required as we track speed elsewhere for the speedometer. Remove before final release.
    float VelocityCalculator(Vector3 newPosition, Vector3 oldPosition, float newTimeStamp, float oldTimeStamp)
    {
        float velocityA = 0;
        distanceTravelledMath = newPosition - oldPosition;
        timePassed = newTimeStamp - oldTimeStamp;
        distanceTravelled = distanceTravelledMath.magnitude;
        velocityA = distanceTravelled * timePassed;

        return velocityA;
    }
}
