using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    //Speed multipliers values editable in unity editor.
    public float accelerationVar = 1;
    public float yawSpeedVar = 100;
    //public float rollSpeedVar = 1;
    //public float maxSpeed = 10;
    //public float minSpeed = -10;

    //Default speed values when active.
    float accelerationMult = 0;
    float yawSpeedMult = 0;
    //float rollSpeedMult = 0;
    float acceleration = 0;
    float yawSpeed = 0;
    Vector3 yawAngularVelocity = new Vector3(0, 0, 0);
    Rigidbody vehicleRB;

    //Add strafe?

    //GetComponent<Rigidbody>().AddForce(Vector3.forward * variable, ForceMode.Acceleration);

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
        acceleration = SpeedSet(accelerationMult, accelerationVar);
        vehicleRB.AddForce(Vector3.forward * acceleration, ForceMode.Acceleration);

        //Yaw stuff still needs to be cleaned up, sorry.
        yawSpeed = SpeedSet(yawSpeedMult, yawSpeedVar);
        yawAngularVelocity = Vector3.up * yawSpeed;
        
        
        Quaternion deltaRotation = Quaternion.Euler(yawAngularVelocity * Time.fixedDeltaTime);
        vehicleRB.MoveRotation(vehicleRB.rotation * deltaRotation);

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
        float acceleration = 0;

        acceleration = multiplier * variable;
        Debug.Log(acceleration);

        return acceleration;
    }
}
