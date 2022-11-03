using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    //Speed multipliers values editable in unity editor.
    public float accelerationVar = 1;
    public float yawSpeedVar = 1;
    //public float rollSpeedVar = 1;
    //public float maxSpeed = 10;
    //public float minSpeed = -10;

    //Default speed values when active.
    float accelerationMult = 0;
    float yawSpeedMult = 0;
    //float rollSpeedMult = 0;
    float acceleration = 0;
    //Add strafe?

    //GetComponent<Rigidbody>().AddForce(Vector3.forward * variable, ForceMode.Acceleration);

    void Update()
    {
        accelerationMult = acceleratorInputCheck();
    }

    void FixedUpdate()
    {
        acceleration = accelerationSet(accelerationMult, accelerationVar);
        GetComponent<Rigidbody>().AddForce(Vector3.forward * acceleration, ForceMode.Acceleration);
    }

    

    float acceleratorInputCheck()
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
    //void yawInputCheck()
    //{

    //}
    float accelerationSet(float accelerationMult, float accelerationVar)
    {
        float acceleration = 0;

        acceleration = accelerationMult * accelerationVar;
        Debug.Log(acceleration);

        return acceleration;
    }
}
