using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Maybe just do this in AntiGravManager, you have a bunch of methods made in there that will help anyway.
public class PitchAndRollManager : MonoBehaviour
{
    private Rigidbody vehicleRB;


    private void Start()
    {
        vehicleRB = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {



    } 
}
