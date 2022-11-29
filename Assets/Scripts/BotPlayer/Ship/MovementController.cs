using Unity.Mathematics;
using UnityEngine;

//Add this script to the vehicle object that has a rigidbody.
public class MovementController : MonoBehaviour
{    
    //Change accelerationVar in ss to adjust forward and backwards speed.
    float accelerationVar;
    //Change yawSpeedVar in ss to adjust yaw rotational speed.
    float yawSpeedVar;
    //accelerationMult and yawMult determine direction as positive or negative numbers.
    float accelerationMult = 0;
    float boostVar;
    float boostMult;
    float yawSpeedMult = 0;
    float torqueLimit;
    float acceleration = 0;
    float yawSpeed = 0;
    Vector3 yawAngularVelocity = new Vector3(0, 0, 0);
    public float reverseLimit = 0.1f;

    float velocity;
    public Quaternion tiltMaxLeft;
    public Quaternion tiltMaxRight;


    Rigidbody vehicleRB;
    ShipsScriptable ss;

    //AudioClip engineSound;
    AudioSource shipAudioSource;
    public AudioClip boostAudio;
    float engineDefaultPitch = 1.0f;
    float pitchLimiter = 0.9f;
    AudioEchoFilter echoFilter;


    public void Reset() 
    {
        accelerationMult = 0;
        yawSpeedMult = 0;
        acceleration = 0;
        yawSpeed = 0;
        yawAngularVelocity = Vector3.zero;
        vehicleRB.velocity = Vector3.zero;
        vehicleRB.angularVelocity = Vector3.zero;
        shipAudioSource.pitch = engineDefaultPitch;
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
        boostAudio = GetComponent<AudioClip>();
        shipAudioSource.Play();
        echoFilter = GetComponent<AudioEchoFilter>();
    }

    void FixedUpdate()
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

        ThrustAudio();        
    }

    public void ThrustController(float thrustInput, float boostInput)
    {
        boostVar = BoostEnabler(thrustInput, boostInput);
        accelerationMult = thrustInput * (boostVar + boostMult);
        if (accelerationMult < 0)
        {
            accelerationMult = accelerationMult * reverseLimit;
        }        
    }

    public void YawController(float yawInput)
    {
        yawSpeedMult = yawInput;
    }

    float SpeedSet(float multiplier, float variable)
    {
        float speed;
        speed = multiplier * variable;

        return speed;
    }

    float BoostEnabler(float tInput, float bInput)
    {
        float newBoost;
        if (tInput > 0 && bInput > 0)
        {
             newBoost = bInput;
        }
        else
        {
            newBoost = 0;
        }

        return newBoost;
    }

    void ThrustAudio()
    {        
        if (shipAudioSource != null)
        {
            //Changes engine sound pitch with velocity.
            shipAudioSource.pitch = engineDefaultPitch + (pitchLimiter * velocity * Time.fixedDeltaTime);

            //Adds echo if ship is moving forward and boosting.
            if (boostVar * velocity > 0)
            {
                echoFilter.enabled = true;
            }
            else
            {
                echoFilter.enabled = false;
            }
        }        
    }
}
