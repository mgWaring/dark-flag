using System;
using System.Collections.Generic;
using UnityEngine;

//Add this script to the vehicle object that has a rigidbody and the colliders.
//Forces applied by the AntiGravManager are affected by the rigidbody mass.
public class AntiGravManager : MonoBehaviour
{
    //+++++++++++++++++ITS A MESS RIGHT NOW, DONT LOOK++++++++++++++++++++
    ShipsScriptable ss;

    //Unity editor options.//
    public bool aGMRaysOn = true;
    public float debugRayTime = 0.1f;    

    Rigidbody vehicleRB;

    //Collider finding stuff.
    Collider[] colliderList;
    Vector3[] colliderSizes;
    Vector3 ultimateVector;
    Vector3 centerOffset;

    //y axis force.
    float hoverForce;
    float hoverConstant;
    RaycastHit hoverHitInfo;
    //float distanceToFloor;      

    //z axis torque.
    Ray rollRay11;
    Ray rollRay21;
    float rollDiff;
    float rollRayDistance1;
    float rollForce;
    RaycastHit rollHit11;
    RaycastHit rollHit21;
    float rollHitInfo11;
    float rollHitInfo21;

    //x axis torque.
    Ray pitchRay1;
    Ray pitchRay2;
    Ray pitchRay3;
    float pitchDiff;
    float pitchRayDistance;
    float pitchForce;
    float pitchRollConstant;
    RaycastHit pitchHit1;
    RaycastHit pitchHit2;
    float pitchHitInfo1;
    float pitchHitInfo2;  
    
    void Start()
    {
        ss = GetComponent<Ship>().details;
        vehicleRB = GetComponent<Rigidbody>();        

        //Collider extent finding.
        colliderList = GetComponents<Collider>();
        colliderSizes = ExtentHunter();
        ultimateVector = ExtentFighter();

        //Gets information from relevent ShipsScriptable.
        hoverForce = ss.hoverForce;
        hoverConstant = ss.hoverConstant;
        rollForce = ss.rollForce;
        rollRayDistance1 = ss.rollRayDistance;
        pitchForce = ss.pitchForce;
        pitchRayDistance = ss.pitchRayDistance;
        pitchRollConstant = ss.pitchRollConstant;
        centerOffset = ss.colRayOffset;
    }

    private void FixedUpdate()
    {
        rollRay11 = new Ray(transform.localPosition + (transform.right * (ultimateVector.x - centerOffset.x)), transform.up * -1);
        rollRay21 = new Ray(transform.localPosition - (transform.right * (ultimateVector.x + centerOffset.x)), transform.up * -1);

        pitchRay1 = new Ray(transform.localPosition + (transform.forward * (ultimateVector.z - centerOffset.z)), (transform.up * -1));
        pitchRay2 = new Ray(transform.localPosition - (transform.forward * (ultimateVector.z + centerOffset.z)), (transform.up * -1));
        //pitchRay3 = new Ray(transform.localPosition + (transform.forward * (ultimateVector.z - centerOffset.z)), (transform.up * -1) + (transform.forward * 2f));

        //Draws all rays for dev purposes.
        if (aGMRaysOn)
        {
            Debug.DrawRay(rollRay11.origin, rollRay11.direction * rollRayDistance1, Color.blue, debugRayTime, true);
            Debug.DrawRay(rollRay21.origin, rollRay21.direction * rollRayDistance1, Color.blue, debugRayTime, true);

            Debug.DrawRay(pitchRay1.origin, pitchRay1.direction * pitchRayDistance, Color.red, debugRayTime, true);
            Debug.DrawRay(pitchRay2.origin, pitchRay2.direction * pitchRayDistance, Color.red, debugRayTime, true);
            //Debug.DrawRay(pitchRay3.origin, pitchRay3.direction * pitchRayDistance, Color.red, debugRayTime, true);
        }

        if ((Physics.Raycast(rollRay11, out rollHit11, rollRayDistance1)) && (Physics.Raycast(rollRay21, out rollHit21, rollRayDistance1)))
        {
            rollHitInfo11 = rollHit11.distance;
            rollHitInfo21 = rollHit21.distance;
            rollDiff = rollHitInfo21 - rollHitInfo11;
            vehicleRB.AddRelativeTorque(Vector3.forward * RollPitchSmoother(rollDiff) * rollForce * Time.fixedDeltaTime, ForceMode.Impulse);
        }

        if ((Physics.Raycast(pitchRay1, out pitchHit1, pitchRayDistance)) && (Physics.Raycast(pitchRay2, out pitchHit2, pitchRayDistance)))
        {
            //y force.
            vehicleRB.AddRelativeForce(Vector3.up * (HoverSmoother(new Ray[] { pitchRay1, pitchRay2/*, pitchRay3*/ }) * Time.fixedDeltaTime), ForceMode.Impulse);
            //x torque.
            pitchHitInfo1 = pitchHit1.distance;
            pitchHitInfo2 = pitchHit2.distance;
            pitchDiff = pitchHitInfo1 - pitchHitInfo2;//May need to swap these around with the smoother in place. Could also put a lot more of this math in RollPitchSmoother.
            vehicleRB.AddRelativeTorque(Vector3.right * RollPitchSmoother(pitchDiff) * pitchForce * Time.fixedDeltaTime, ForceMode.Impulse);
        }
    }

    float HoverSmoother(Ray[] inputRays)
    {
        float distanceToFloor = 0.0f;

        //Combines distance to floor of all rays passed in, then calculates the mean.
        for (int i = 0; i < inputRays.Length; i++)
        {
            Physics.Raycast(inputRays[i], out hoverHitInfo, pitchRayDistance);
            distanceToFloor = distanceToFloor + hoverHitInfo.distance;
        }
        float averageDistance = distanceToFloor / inputRays.Length;

        //regulator increases as distance to the floor lowers. Can be adjusted by changing hoverConstant.
        float regulator = hoverConstant / (averageDistance + 0.1f);
        float newForce = hoverForce * regulator;
        return newForce;
    }

    float RollPitchSmoother(float distanceDifference)
    {
        float newForce = 0.0f;

        if (distanceDifference != newForce)
        {
            //regulator increases with the difference between pitchRay1 & pitchRay2 distance to hit.
            float regulator = (distanceDifference / pitchRollConstant);
            newForce = hoverForce * regulator;
        }
        return newForce;
    }

    //Finds the extents of all colliders on parent object.
    private Vector3[] ExtentHunter()
    {
        List<Vector3> colliderSizesList = new List<Vector3>();

        for (int i = 0; i < colliderList.Length; i++)
        {
            colliderSizesList.Add(colliderList[i].bounds.extents);
        }

        Vector3[] hunterArray = colliderSizesList.ToArray();
        return hunterArray;
    }

    //Compares extents and keeps the highest for x, y, z.
    public Vector3 ExtentFighter()
    {
        float championZ = 0;
        float championX = 0;
        float championY = 0;

        for (int q = 0; q < colliderSizes.Length; q++)
        {
            float contenderZ = colliderSizes[q].z;
            if (contenderZ > championZ)
            {
                championZ = contenderZ;
            }

            float contenderX = colliderSizes[q].x;
            if (contenderX > championX)
            {
                championX = contenderX;
            }

            float contenderY = colliderSizes[q].y;
            if (contenderY > championY)
            {
                championY = contenderY;
            }
        }

        Vector3 championFusion = new Vector3(championX, championY, championZ);
        return championFusion;
    }
}
