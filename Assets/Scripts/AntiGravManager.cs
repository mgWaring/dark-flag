using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//Add this script to the vehicle object that has a rigidbody and the colliders.
//Forces applied by the AntiGravManager are affected by the rigidbody mass.
public class AntiGravManager : MonoBehaviour
{
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
    Ray rollRay1;
    Ray rollRay2;
    float rollDiff;
    float rollRayDistance;
    float rollForce;
    RaycastHit rollHit1;
    RaycastHit rollHit2;
    float rollHitInfo1;
    float rollHitInfo2;

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

    //self righting.
    bool pitchIsStable;
    bool rollIsStable;
    public float stableForce = 100.0f;//move this to scriptables.
    Ray bowRay;
    RaycastHit bowHit;
    public float bowHitDistance = 10.0f;
    Ray sternRay;
    RaycastHit sternHit;
    public float sternHitDistance = 10.0f;
    Ray starboardRay;
    RaycastHit starboardHit;
    Ray portRay;
    RaycastHit portHit;
    public float portStarHitDistance = 10.0f;
    public Vector3 stableOffsetVector = new Vector3(0.5f,0.5f,0.5f);

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
        rollRayDistance = ss.rollRayDistance;
        pitchForce = ss.pitchForce;
        pitchRayDistance = ss.pitchRayDistance;
        pitchRollConstant = ss.pitchRollConstant;
        centerOffset = ss.colRayOffset;
    }

    private void FixedUpdate()
    {
        rollRay1 = new Ray(transform.localPosition + (transform.right * (ultimateVector.x - centerOffset.x)), transform.up * -1);
        rollRay2 = new Ray(transform.localPosition - (transform.right * (ultimateVector.x + centerOffset.x)), transform.up * -1);

        pitchRay1 = new Ray(transform.localPosition + (transform.forward * (ultimateVector.z - centerOffset.z)), (transform.up * -1));
        pitchRay2 = new Ray(transform.localPosition - (transform.forward * (ultimateVector.z + centerOffset.z)), (transform.up * -1));

        bowRay = new Ray(transform.localPosition + (transform.forward * (ultimateVector.z - centerOffset.z - stableOffsetVector.z)), transform.forward);
        sternRay = new Ray(transform.localPosition - (transform.forward * (ultimateVector.z + centerOffset.z - stableOffsetVector.z)), transform.forward * -1);
        starboardRay = new Ray(transform.localPosition + (transform.right * (ultimateVector.x - centerOffset.x - stableOffsetVector.x)), transform.right);
        portRay = new Ray(transform.localPosition - (transform.right * (ultimateVector.x + centerOffset.x - stableOffsetVector.x)), transform.right * -1);

        //Draws all rays for dev purposes.
        if (aGMRaysOn)
        {
            Debug.DrawRay(rollRay1.origin, rollRay1.direction * rollRayDistance, Color.blue, debugRayTime, true);
            Debug.DrawRay(rollRay2.origin, rollRay2.direction * rollRayDistance, Color.blue, debugRayTime, true);

            Debug.DrawRay(pitchRay1.origin, pitchRay1.direction * pitchRayDistance, Color.red, debugRayTime, true);
            Debug.DrawRay(pitchRay2.origin, pitchRay2.direction * pitchRayDistance, Color.red, debugRayTime, true);

            Debug.DrawRay(bowRay.origin, bowRay.direction * bowHitDistance, Color.red, debugRayTime, true);
            Debug.DrawRay(sternRay.origin, sternRay.direction * sternHitDistance, Color.red, debugRayTime, true);
            Debug.DrawRay(starboardRay.origin, starboardRay.direction * portStarHitDistance, Color.blue, debugRayTime, true);
            Debug.DrawRay(portRay.origin, portRay.direction * portStarHitDistance, Color.blue, debugRayTime, true);
        }

        if (Physics.Raycast(rollRay1, out rollHit1, rollRayDistance) && Physics.Raycast(rollRay2, out rollHit2, rollRayDistance))
        {
            rollIsStable = true;
            //z torque.
            rollHitInfo1 = rollHit1.distance;
            rollHitInfo2= rollHit2.distance;
            //If ship is rolling the wrong way, swap rollHitInfo 2 and rollHitInfo 1 below.
            rollDiff = rollHitInfo2 - rollHitInfo1;
            vehicleRB.AddRelativeTorque(Vector3.forward * RollPitchSmoother(rollDiff) * rollForce * Time.fixedDeltaTime, ForceMode.Impulse);
            Debug.Log("rollIsStable" + rollIsStable);
        }
        else
        {
            rollIsStable = false;
            Debug.Log("rollIsStable" + rollIsStable);
        }

        if (Physics.Raycast(pitchRay1, out pitchHit1, pitchRayDistance) && Physics.Raycast(pitchRay2, out pitchHit2, pitchRayDistance))
        {
            pitchIsStable = true;
            //y force.
            vehicleRB.AddRelativeForce(Vector3.up * (HoverSmoother(new Ray[] { pitchRay1, pitchRay2/*, pitchRay3*/ }) * Time.fixedDeltaTime), ForceMode.Impulse);
            //x torque.
            pitchHitInfo1 = pitchHit1.distance;
            pitchHitInfo2 = pitchHit2.distance;
            //If ship is pitching the wrong way, swap pitchHitInfo 2 and pitchHitInfo 1 below.
            pitchDiff = pitchHitInfo1 - pitchHitInfo2;
            vehicleRB.AddRelativeTorque(Vector3.right * RollPitchSmoother(pitchDiff) * pitchForce * Time.fixedDeltaTime, ForceMode.Impulse);
            Debug.Log("pitchIsStable" + pitchIsStable);
        }
        else
        {
            pitchIsStable = false;
            Debug.Log("pitchIsStable" + pitchIsStable);
        }
    }

    /*void SelfRight()
    {
        if (!pitchThing && rayhit)
        {
            addrelativeforce = Direction * Force
            
        }



    }*/

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
