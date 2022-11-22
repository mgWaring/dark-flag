using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Maybe just do this in AntiGravManager, you have a bunch of methods made in there that will help anyway.
public class PitchAndRollManager : MonoBehaviour
{
    Rigidbody vehicleRB;


    private void Start()
    {
        vehicleRB = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {



    } 
}
