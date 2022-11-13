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
    public Vector3 centerOffset = new Vector3(0, 0, 0);

    Rigidbody vehicleRB;

    //Collider finding stuff.
    Collider[] colliderList;
    Vector3[] colliderSizes;
    Vector3 ultimateVector;

    //y axis force.
    Ray hoverRay1;
    Ray hoverRay2;
    Ray hoverRay3;
    float hoverRayDistance;
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
    RaycastHit pitchHit1;
    RaycastHit pitchHit2;
    float pitchHitInfo1;
    float pitchHitInfo2;
    public float pitchRollConstant = 1.2f;//put in ss when you can figure out how to do that again.



    void Start()
    {
        vehicleRB = GetComponent<Rigidbody>();
        ss = GetComponent<Ship>().details;
        //Collider extent finding.
        colliderList = GetComponents<Collider>();
        colliderSizes = ExtentHunter();
        ultimateVector = ExtentFighter();

        //Gets information from relevent ShipsScriptable.
        hoverForce = ss.hoverForce;
        hoverConstant = ss.hoverConstant;
        hoverRayDistance = ss.hoverHeight;
        rollForce = ss.rollForce;
        rollRayDistance1 = ss.rollRayDistance;
        pitchForce = ss.pitchForce;
        pitchRayDistance = ss.pitchRayDistance;
    }

    private void FixedUpdate()
    {

        hoverRay1 = new Ray(transform.localPosition - (transform.up * ultimateVector.y), transform.up * -1);//might not need this one.

        rollRay11 = new Ray(transform.localPosition + (transform.right * (ultimateVector.x - centerOffset.x)), transform.up * -1);
        rollRay21 = new Ray(transform.localPosition - (transform.right * (ultimateVector.x + centerOffset.x)), transform.up * -1);


        pitchRay1 = new Ray(transform.localPosition + (transform.forward * (ultimateVector.z - centerOffset.z)), (transform.up * -1));
        pitchRay2 = new Ray(transform.localPosition - (transform.forward * (ultimateVector.z + centerOffset.z)), (transform.up * -1));
        //pitchRay3 = new Ray(transform.localPosition + (transform.forward * (ultimateVector.z - centerOffset.z)), (transform.up * -1) + (transform.forward * 2f));

        //Draws all rays for dev purposes.
        if (aGMRaysOn)
        {
            Debug.DrawRay(hoverRay1.origin, hoverRay1.direction * hoverRayDistance, Color.green, debugRayTime, true);

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
            //Debug.Log("rollDiff = " + rollDiff + " rollHitInfo1 = " + rollHitInfo11 + " rollHitInfo2 = " + rollHitInfo21);
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

            //IDebug.Log("pitchDiff = " + pitchDiff + " pitchHitInfo1 = " + pitchHitInfo1 + " pitchHitInfo2 = " + pitchHitInfo2);
        }


    }

    float HoverSmoother(Ray[] inputRays)
    {
        float distanceToFloor = 0.0f;

        //Combines distance to floor of all rays passed in, then calculates the mean.
        for (int i = 0; i < inputRays.Length; i++)
        {
            Physics.Raycast(inputRays[i], out hoverHitInfo, hoverRayDistance);
            distanceToFloor = distanceToFloor + hoverHitInfo.distance;
        }
        float averageDistance = distanceToFloor / inputRays.Length;

        //regulator increases as distance to the floor lowers. Can be adjusted by changing hoverConstant.
        float regulator = hoverConstant / (averageDistance + 0.1f);
        float newForce = hoverForce * regulator;

        //Debug.Log("From HoverSmoother in AntiGravManager. newHoverForce = " + newHoverForce);
        return newForce;
    }

    float RollPitchSmoother(float distanceDifference)
    {
        float newForce = 0.0f;

        if (distanceDifference != newForce)
        {
            float regulator = (distanceDifference / pitchRollConstant);
            newForce = hoverForce * regulator;
        }
        Debug.Log("newForce = " + newForce);
        return newForce;
    }


    private Vector3[] ExtentHunter()
    {
        List<Vector3> colliderSizesList = new List<Vector3>();

        for (int i = 0; i < colliderList.Length; i++)
        {
            colliderSizesList.Add(colliderList[i].bounds.extents);
            Debug.Log("From SizeHunter in ColliderInfo.cs. " + colliderSizesList[i] + " added to colliserSizesList.");
        }

        Vector3[] hunterArray = colliderSizesList.ToArray();
        return hunterArray;
    }

    public Vector3 ExtentFighter()
    {
        float championZ = 0;
        float championX = 0;
        float championY = 0;

        for (int q = 0; q < colliderSizes.Length; q++)
        {
            float contenderZ = colliderSizes[q].z;
            Debug.Log("ContenderZ attacks " + championZ + "with" + contenderZ);
            if (contenderZ > championZ)
            {
                championZ = contenderZ;
                Debug.Log("New championZ is " + championZ);
            }
            else
            {
                Debug.Log("ChampionZ holds their title at value " + championZ);
                //Delete else clause when done debugging.
            }

            float contenderX = colliderSizes[q].x;
            Debug.Log("ContenderX attacks " + championX + "with" + contenderX);
            if (contenderX > championX)
            {
                championX = contenderX;
                Debug.Log("New championX is " + championX);
            }
            else
            {
                Debug.Log("ChampionX holds their title at value " + championX);
                //Delete else clause when done debugging.
            }

            float contenderY = colliderSizes[q].y;
            Debug.Log("ContenderY attacks " + championY + "with" + contenderY);
            if (contenderY > championY)
            {
                championY = contenderY;
                Debug.Log("New championY is " + championY);
            }
            else
            {
                Debug.Log("ChampionY holds their title at value " + championY);
                //Delete else clause when done debugging.
            }
        }

        Vector3 championFusion = new Vector3(championX, championY, championZ);
        Debug.Log("Your new ultimate Vector is " + championFusion);
        return championFusion;
    }
}
