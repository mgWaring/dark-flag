using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System;
using UnityEngine;
using System.Threading;
using Unity.Mathematics;

//Add this script to the vehicle object that has a rigidbody.
public class MovementController : MonoBehaviour
{
    //Change accelerationVar to adjust forward and backwards speed.
    public float accelerationVar = 6000;
    //Change yawSpeedVar to adjust yaw rotational speed.
    public float yawSpeedVar = 4;

    public float rollSpeedVar = 1;

    //accelerationMult and yawMult determine direction when they are either +1 or -1. Cuts power when they are 0.
    float accelerationMult = 0;
    float yawSpeedMult = 0;
    
    float acceleration = 0;
    float yawSpeed = 0;
    Vector3 yawAngularVelocity = new Vector3(0, 0, 0);
    float rollSpeed = 0;
    Vector3 rollAngularVelocity = new Vector3(0, 0, 0);

    Rigidbody vehicleRB;

    bool movementInputCheck = false;

    Vector3 oldPosition;
    Vector3 newPosition;
    Vector3 distanceTravelledMath;
    float newTimeStamp;
    float oldTimeStamp;
    float timePassed;
    float distanceTravelled = 0;
    float vehicleVelocity;

    
   

    //Add strafe?

    private void Start()
    {
        vehicleRB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        accelerationMult = AcceleratorInputCheck();
        yawSpeedMult = YawInputCheck();
    }

    void FixedUpdate()
    {
        
        newPosition = transform.position;
        newTimeStamp = Time.realtimeSinceStartup;
        vehicleVelocity = VelocityCalculator(newPosition, oldPosition,  newTimeStamp, oldTimeStamp);

        acceleration = SpeedSet(accelerationMult, accelerationVar);
        vehicleRB.AddRelativeForce(Vector3.forward * acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);

        oldPosition = transform.position;
        oldTimeStamp = Time.realtimeSinceStartup;

        
        yawSpeed = SpeedSet(yawSpeedMult, yawSpeedVar);
        yawAngularVelocity = Vector3.up * yawSpeed;
        Quaternion yawDeltaRotation = Quaternion.Euler(yawAngularVelocity * Time.fixedDeltaTime);
        vehicleRB.MoveRotation(vehicleRB.rotation * yawDeltaRotation);




        //rollSpeed = rollSpeedVar * vehicleVelocity * accelerationMult * yawSpeedMult;
        //rollAngularVelocity = Vector3.forward * rollSpeed;
        //Quaternion rollDeltaRotation = Quaternion.Euler(rollAngularVelocity * Time.fixedDeltaTime);
        //vehicleRB.MoveRotation(vehicleRB.rotation * rollDeltaRotation);

        //Debug.Log(transform.position);
        //Debug.Log(newPosition + " - " + currentPosition + " = " + distanceTravelled);
        //Debug.Log(Math.Round(newSpeed*1000, 2));
        //Debug.Log("Speed = " + Math.Round(distanceTravelled*1000, 0));
        //Debug.Log(vehicleRB.velocity);
        Debug.Log("Speed = " + Math.Round(vehicleVelocity*1000, 0));
        //Debug.Log(rollSpeed);
    }



    float AcceleratorInputCheck()
    {
        float accelerationMult = 0;
        //Checks for forward or backwards input.
        if (Input.GetKey(KeyCode.W))
        {
            accelerationMult = accelerationMult + 1;
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            accelerationMult = accelerationMult - 1;
        }

        return accelerationMult;
    }

    float YawInputCheck()
    {
        float yawMult = 0;
        //Checks for left or right input.
        if (Input.GetKey(KeyCode.D))
        {
            yawMult = yawSpeedMult + 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            yawMult = yawSpeedMult - 1;
        }

        return yawMult;
    }

    float SpeedSet(float multiplier, float variable)
    {
        float speed = 0;

        speed = multiplier * variable;
        //Debug.Log(acceleration);

        return speed;
    }

    bool MoveInputCheck(float accelerationMult, float yawSpeedMult)
    {
        bool inputCheck = false;
        if (accelerationMult + yawSpeedMult != 0)
        {
            inputCheck = true;
        }


        return inputCheck;
    }

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
