using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float accelerationVar = 10;
    public float yawSpeedVar = 10;
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
        acceleration = SpeedSet(accelerationMult, accelerationVar);
        vehicleRB.AddRelativeForce(Vector3.forward * acceleration * Time.fixedDeltaTime, ForceMode.Force);

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
        float speed = 0;

        speed = multiplier * variable;
        Debug.Log(acceleration);

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
