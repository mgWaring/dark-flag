using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System;
using UnityEngine;

//Add this script to the vehicle object that has a rigidbody.
public class MovementController : MonoBehaviour
{
    //Change accelerationVar to adjust forward and backwards speed.
    public float accelerationVar = 6000;
    //Change yawSpeedVar to adjust yaw rotational speed.
    public float yawSpeedVar = 5;

    //public float rollSpeedVar = 1;
    //public float maxSpeed = 10;
    //public float minSpeed = -10;

    float accelerationMult = 0;
    float yawSpeedMult = 0;
    //float rollSpeedMult = 0;
    
    float acceleration = 0;
    float yawSpeed = 0;
    Vector3 yawAngularVelocity = new Vector3(0, 0, 0);
    
    Rigidbody vehicleRB;

    bool movementInputCheck = false;

    Vector3 oldPosition;
    Vector3 newPosition;
    Vector3 distanceTravelledMath;
    float newTimeStamp;
    float oldTimeStamp;
    float timePassedMath;
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
        distanceTravelledMath = newPosition - oldPosition;
        timePassedMath = newTimeStamp - oldTimeStamp;
        distanceTravelled = distanceTravelledMath.magnitude;
        vehicleVelocity = distanceTravelled * timePassedMath;


        //
        MoveInputCheck(accelerationMult, yawSpeedMult);
        acceleration = SpeedSet(accelerationMult, accelerationVar);
        vehicleRB.AddRelativeForce(Vector3.forward * acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);
        
        //

        oldPosition = transform.position;
        oldTimeStamp = Time.realtimeSinceStartup;
        

        //Yaw stuff still needs to be cleaned up, sorry.
        yawSpeed = SpeedSet(yawSpeedMult, yawSpeedVar);
        yawAngularVelocity = Vector3.up * yawSpeed;
        Quaternion deltaRotation = Quaternion.Euler(yawAngularVelocity * Time.fixedDeltaTime);
        vehicleRB.MoveRotation(vehicleRB.rotation * deltaRotation);


        //Debug.Log(transform.position);
        //Debug.Log(newPosition + " - " + currentPosition + " = " + distanceTravelled);
        //Debug.Log(Math.Round(newSpeed*1000, 2));
        //Debug.Log("Speed = " + Math.Round(distanceTravelled*1000, 0));
        //Debug.Log(vehicleRB.velocity);
        Debug.Log("Speed = " + Math.Round(vehicleVelocity*1000, 0));

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
}
