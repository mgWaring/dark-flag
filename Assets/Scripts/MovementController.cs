using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System;
using UnityEngine;
using System.Threading;
using Unity.Mathematics;
using System.ComponentModel;
using System.Net.NetworkInformation;

//Add this script to the vehicle object that has a rigidbody.
public class MovementController : MonoBehaviour
{
    //Change accelerationVar to adjust forward and backwards speed.
    public float accelerationVar = 6000f;
    //Change yawSpeedVar to adjust yaw rotational speed.
    public float yawSpeedVar = 4.3f;

    //accelerationMult and yawMult determine direction when they are either +1 or -1. Cuts power when they are 0.
    float accelerationMult = 0;
    float yawSpeedMult = 0;
    string accDir = "";
    string yawDir = "";
    
    float acceleration = 0;
    float yawSpeed = 0;
    Vector3 yawAngularVelocity = new Vector3(0, 0, 0);


    float rollSpeed = 0;
    public float rollSpeedVar = 1;
    Vector3 rollTorque = new Vector3(0, 0, 0);



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
    
    //public Vector3 offset = new Vector3(0f, 0.5f, 0.8f);
    //public float rayDistance = 10;

    Collider vehicleCollider;

    
    //Transform directionVector = transform.position;
    //Vector3 relativeDirection = new Vector3(0, 0, 0);
    //Vector3 relativeDirection = directionVector.InverseTransformPoin(transform.position);




    //Add strafe?

    private void Start()
    {
        vehicleRB = GetComponent<Rigidbody>();
        vehicleCollider = GetComponent<CapsuleCollider>();

    }

    void Update()
    {
        accelerationMult = AcceleratorInputCheck();
        yawSpeedMult = YawInputCheck();
        yawDir = "";
        accDir = "";

        //Vector3 relativePoint = transform.InverseTransformPoint(0, 0.5f, 0.8f);
        //RaycastHit hit;
        //Ray testRay = new Ray(transform.localPosition + offset, transform.forward);
        //Debug.DrawRay(vehicleCollider.transform.localPosition + offset, vehicleCollider.transform.forward, Color.red, 9999999, true); ;
        //if (Physics.Raycast(testRay, out hit, rayDistance))
        //{

            //Debug.Log("I've hit " + hit.collider);
        //}
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

        


        //will roll in the wrong direction if going backwards.
        //rollSpeed =  accelerationMult * (yawSpeedMult * -1) * vehicleVelocity * rollSpeedVar;
        //vehicleRB.AddRelativeTorque(Vector3.forward * rollSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);





        //rollSpeed = rollSpeedVar * vehicleVelocity * accelerationMult * yawSpeedMult;
        //rollAngularVelocity = Vector3.forward * rollSpeed;
        //Quaternion rollDeltaRotation = Quaternion.Euler(rollAngularVelocity * Time.fixedDeltaTime);
        //vehicleRB.MoveRotation(vehicleRB.rotation * rollDeltaRotation);

        //Debug.Log(transform.position);
        //Debug.Log(newPosition + " - " + currentPosition + " = " + distanceTravelled);
        //Debug.Log(Math.Round(newSpeed*1000, 2));
        //Debug.Log("Speed = " + Math.Round(distanceTravelled*1000, 0));
        //Debug.Log(vehicleRB.velocity);
        //Debug.Log("Speed = " + Math.Round(vehicleVelocity*1000, 0));
        //Debug.Log(rollSpeed);
        //Debug.Log(oldTimeStamp);

    }

    public void MoveForward() {
        accDir = "forward";
    }

    public void MoveBackward() {
        accDir = "backward";
    }

    public void MoveRight() {
        yawDir = "right";
    }

    public void MoveLeft() {
        yawDir = "left";
    }

    float AcceleratorInputCheck()
    {
        float accelerationMult = 0;
        //Checks for forward or backwards input.
        if (accDir == "forward")
        {
            accelerationMult = accelerationMult + 1;
        }
        
        if (accDir == "backward")
        {
            accelerationMult = accelerationMult - 1;
        }

        return accelerationMult;
    }

    float YawInputCheck()
    {
        float yawMult = 0;
        //Checks for left or right input.
        if (yawDir == "right")
        {
            yawMult = yawSpeedMult + 1;
        }

        if (yawDir == "left")
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
