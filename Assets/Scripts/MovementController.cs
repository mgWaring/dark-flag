using UnityEngine;

//Add this script to the vehicle object that has a rigidbody.
public class MovementController : MonoBehaviour
{    
    //Change accelerationVar in ss to adjust forward and backwards speed.
    private float accelerationVar;
    //Change yawSpeedVar in ss to adjust yaw rotational speed.
    private float yawSpeedVar;
    //accelerationMult and yawMult determine direction as positive or negative numbers.
    private float accelerationMult = 0;
    private float boostMult;
    private float yawSpeedMult = 0;
    private float torqueLimit;
    private float acceleration = 0;
    private float yawSpeed = 0;
    private Vector3 yawAngularVelocity = new Vector3(0, 0, 0);

    private float velocity;
    public Quaternion tiltMaxLeft;
    public Quaternion tiltMaxRight;


    private Rigidbody vehicleRB;

    private ShipsScriptable ss;
    //AudioClip engineSound;
    private AudioSource shipAudioSource;
    private float engineDefaultPitch = 1.0f;
    private float pitchLimiter = 0.9f;

    public void Reset() 
    {
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
        ss = GetComponent<Ship>().details;
        vehicleRB = GetComponent<Rigidbody>();
        accelerationVar = ss.thrustSpeed;
        boostMult = ss.boost;
        yawSpeedVar = ss.yawSpeed;
        torqueLimit = ss.torqueLimiter;
        tiltMaxLeft = ss.turnTiltMaximum;
        tiltMaxRight = Quaternion.Euler(0.0f, 0.0f, 360.0f - tiltMaxLeft.z);
        shipAudioSource = GetComponent<AudioSource>();
        //engineSound = ss.engineIdleSound;
        shipAudioSource.Play();
        

    }

    private void FixedUpdate()
    {
        velocity = vehicleRB.velocity.magnitude;

        //Adds thrust force to vehicle.
        acceleration = SpeedSet(accelerationMult, accelerationVar);
        vehicleRB.AddRelativeForce(Vector3.forward * acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);        

        //Rotates vehicle.
        yawSpeed = SpeedSet(yawSpeedMult, yawSpeedVar);
        yawAngularVelocity = Vector3.up * yawSpeed;
        Quaternion yawDeltaRotation = Quaternion.Euler(yawAngularVelocity * Time.fixedDeltaTime);
        vehicleRB.MoveRotation(vehicleRB.rotation * yawDeltaRotation);
        //Adds a tilt when turning. Leans more at higher speeds. Adjust torqueLimiter in ss to change tilt amount.
        if (vehicleRB.rotation.eulerAngles.z <= tiltMaxLeft.z || vehicleRB.rotation.eulerAngles.z >= tiltMaxRight.z)
        {
            vehicleRB.AddRelativeTorque(Vector3.forward * (yawSpeedMult * -1) * (velocity * torqueLimit), ForceMode.Force);
        }
        
    }

    public void ThrustController(float thrustInput, float boostInput)
    {
        accelerationMult = thrustInput * (boostInput + boostMult);
        shipAudioSource.pitch = engineDefaultPitch + (pitchLimiter * velocity * Time.fixedDeltaTime);
    }

    public void YawController(float yawInput)
    {
        yawSpeedMult = yawInput;
    }

    private float SpeedSet(float multiplier, float variable)
    {
        float speed;
        speed = multiplier * variable;

        return speed;
    }
}
