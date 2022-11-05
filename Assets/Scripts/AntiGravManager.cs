using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGravManager : MonoBehaviour
{
    Rigidbody vehicleRB;
    Collider vehicleCL;
        
    public Vector3 hoverRayYOffset = new Vector3(0.0f, -1.0f, 0.0f);
    public float hoverRayDistance = 1.0f;

    public float hoverForce = 10;

    RaycastHit hitInfo;
    LayerMask hoverMask;

    Ray hoverRay1;
    Vector3 hoverRay1XZOffset = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 hoverRay1CombOffset;

    void Start()
    {
        vehicleRB = GetComponent <Rigidbody>();
        vehicleCL = GetComponent <CapsuleCollider>();
        hoverMask = LayerMask.GetMask("Ship");
        hoverRay1CombOffset = OffsetCombiner(hoverRay1XZOffset);




    }

    private void FixedUpdate()
    {
        hoverRay1 = new Ray(transform.localPosition , transform.up * -1);
        Debug.DrawRay(transform.localPosition, transform.up * 10 * -1, Color.red, 9999, true);
        //Debug.DrawRay(vehicleCollider.transform.localPosition + offset, vehicleCollider.transform.forward, Color.red, 9999999, true); ;

        if (GroundDetector(hoverRay1))
        {
            vehicleRB.AddRelativeForce(Vector3.up * hoverForce, ForceMode.Force);
        }
    }

    //Add more inputs and GroundDetector() calls as neccesary.
    void DetectorManager(Ray inputRay1)
    {
        GroundDetector(inputRay1);
    }
    
    bool GroundDetector(Ray inputRay)
    {
        bool groundDetected = false;

        if (Physics.Raycast(inputRay, out hitInfo, hoverRayDistance))
        {
            groundDetected = true;
            Debug.Log(hitInfo.collider + " detected by GroundDetector() in AntiGravManager");
        }

        if (Physics.Raycast(inputRay, out hitInfo, hoverRayDistance, hoverMask))
        {
            groundDetected = false;
            Debug.Log("Ship detected by GroundDetector() in AntiGravManager");
        }

        return groundDetected;
    }

    Vector3 OffsetCombiner(Vector3 xzOffset)
    {
        Vector3 combinedOffset = new Vector3(0, 0, 0);
        combinedOffset = hoverRayYOffset + xzOffset;

        Debug.Log(combinedOffset);
        return combinedOffset;
    }


}
