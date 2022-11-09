using UnityEngine;

//Add this script to the vehicle object that has a rigidbody.
//Forces applied by the AntiGravManager are affected by the rigidbody mass.
public class AntiGravManager : MonoBehaviour
{    
    Rigidbody vehicleRB;
    Collider vehicleCL;//Not used at the moment. Delete if still unused before final release of game.

    public Vector3 hoverRayYOffset = new Vector3(0.0f, -1.0f, 0.0f);//Not used at the moment. Delete if still unused before final release of game.
    float hoverRayDistance;
    float hoverForce;
    float hoverConstant;
    
    float distanceToFloor;

    RaycastHit hoverHitInfo;
    LayerMask shipLayer;

    Ray hoverRay1;    
    Vector3 hoverRay1XZOffset = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 hoverRay1CombOffset;//Not used at the moment. Delete if still unused before final release of game.

    Ray rollRay1;
    Ray rollRay2;
    float rollDiff;
    float rollRayDistance;
    float rollForce;
    RaycastHit rollHit1;
    RaycastHit rollHit2;
    float rollHitInfo1;
    float rollHitInfo2;

    Ray pitchRay1;
    Ray pitchRay2;
    float pitchDiff;
    float pitchRayDistance;
    float pitchForce;
    RaycastHit pitchHit1;
    RaycastHit pitchHit2;
    float pitchHitInfo1;
    float pitchHitInfo2;

    public float debugRayTime = 0.1f;

    [HideInInspector] public ShipsScriptable ss;

    void Start()
    {
        vehicleRB = GetComponent <Rigidbody>();
        vehicleCL = GetComponent <CapsuleCollider>();//Not used at the moment. Delete if still unused before final release of game.
        //hoverMask = LayerMask.GetMask("Ship");
        hoverRay1CombOffset = OffsetCombiner(hoverRay1XZOffset);//Not used at the moment. Delete if still unused before final release of game.
        
        hoverForce = ss.hoverForce;
        hoverConstant = ss.hoverConstant;
        hoverRayDistance = ss.hoverHeight;
        rollForce = ss.rollForce;
        rollRayDistance = ss.rollRayDistance;
        pitchForce = ss.pitchForce;
        pitchRayDistance = ss.pitchRayDistance;

    }

    private void FixedUpdate()
    {
        hoverRay1 = new Ray(transform.localPosition, transform.up * -1);
        rollRay1 = new Ray(transform.localPosition,  (transform.up + transform.right) * -1);
        rollRay2 = new Ray(transform.localPosition, (transform.up + (transform.right * -1)) * -1);
        pitchRay1 = new Ray(transform.localPosition, (transform.up + transform.forward) * -1);
        pitchRay2 = new Ray(transform.localPosition, (transform.up + (transform.forward * -1)) * -1);
        //Leave these comments here please. Delete if still unused before final release of game.
        Debug.DrawRay(transform.localPosition, transform.up * hoverRayDistance * -1, Color.green, debugRayTime, true);
        //Debug.DrawRay(vehicleCollider.transform.localPosition + offset, vehicleCollider.transform.forward, Color.red, 9999999, true);
        Debug.DrawRay(transform.localPosition, (transform.up + transform.right)* rollRayDistance * -1, Color.blue, debugRayTime, true);
        Debug.DrawRay(transform.localPosition, (transform.up + (transform.right * -1)) * rollRayDistance * -1, Color.blue, debugRayTime, true);
        Debug.DrawRay(transform.localPosition, (transform.up + transform.forward) * pitchRayDistance * -1, Color.red, debugRayTime, true);
        Debug.DrawRay(transform.localPosition, (transform.up + (transform.forward * -1)) * pitchRayDistance * -1, Color.red, debugRayTime, true);

        if (Physics.Raycast(hoverRay1, out hoverHitInfo, hoverRayDistance))
        {
            vehicleRB.AddRelativeForce(Vector3.up * HoverSmoother(hoverRay1), ForceMode.Force);
        }

        if ((Physics.Raycast(rollRay1, out rollHit1, rollRayDistance)) | (Physics.Raycast(rollRay2, out rollHit2, rollRayDistance)))
        {
            rollHitInfo1 = rollHit1.distance;
            rollHitInfo2 = rollHit2.distance;
            rollDiff = rollHitInfo2 - rollHitInfo1;
            vehicleRB.AddRelativeTorque(Vector3.forward * rollDiff * rollForce * Time.fixedDeltaTime, ForceMode.Force);
            Debug.Log("rollDiff = " + rollDiff + " rollHitInfo1 = " + rollHitInfo1 + " rollHitInfo2 = " + rollHitInfo2);
        }

        if ((Physics.Raycast(pitchRay1, out pitchHit1, pitchRayDistance)) | (Physics.Raycast(pitchRay2, out pitchHit2, pitchRayDistance)))
        {
            pitchHitInfo1 = pitchHit1.distance;
            pitchHitInfo2 = pitchHit2.distance;
            pitchDiff = pitchHitInfo1 - pitchHitInfo2;
            vehicleRB.AddRelativeTorque(Vector3.right * pitchDiff * pitchForce * Time.fixedDeltaTime, ForceMode.Force);
            Debug.Log("pitchDiff = " + pitchDiff + " pitchHitInfo1 = " + pitchHitInfo1 + " pitchHitInfo2 = " + pitchHitInfo2);
        }
    }
    
    bool GroundDetector(Ray inputRay)
    {
        bool groundDetected = false;

        if (Physics.Raycast(inputRay, out hoverHitInfo, hoverRayDistance))
        {
            groundDetected = true;
        }

        return groundDetected;
    }



    float HoverSmoother(Ray inputRay)
    {
        float newHoverForce = 0.0f;
        float hoverRegulator;

        Physics.Raycast(inputRay, out hoverHitInfo, hoverRayDistance);//Maybe ass the mask to this one.
        distanceToFloor = hoverHitInfo.distance;
        //hoverRegulator increases as distance to the floor lowers. Can be adjusted by changing hoverConstant.
        hoverRegulator = (hoverConstant / (distanceToFloor + 1));
        newHoverForce = hoverForce * hoverRegulator;

        //Debug.Log("From HoverSmoother in AntiGravManager. newHoverForce = " + newHoverForce);
        return newHoverForce;
    }

    Vector3 OffsetCombiner(Vector3 xzOffset)
    {
        Vector3 combinedOffset = new Vector3(0, 0, 0);
        combinedOffset = hoverRayYOffset + xzOffset;

        //Debug.Log(combinedOffset);
        return combinedOffset;
    }
}
