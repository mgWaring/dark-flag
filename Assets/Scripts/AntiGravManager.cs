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

    Ray rollRay11;
    Ray rollRay12;
    Ray rollRay21;
    Ray rollRay22;
    float rollDiff;
    float rollRayDistance1;
    float rollRayDistance2;
    float rollForce;
    RaycastHit rollHit11;
    RaycastHit rollHit12;
    RaycastHit rollHit21;
    RaycastHit rollHit22;
    float rollHitInfo11;
    float rollHitInfo12;
    float rollHitInfo21;
    float rollHitInfo22;

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
    Collider mCollider;
    Vector3 minCollider;

    [HideInInspector] public ShipsScriptable ss;

    void Start()
    {
        vehicleRB = GetComponent <Rigidbody>();
        //hoverMask = LayerMask.GetMask("Ship");
        hoverRay1CombOffset = OffsetCombiner(hoverRay1XZOffset);//Not used at the moment. Delete if still unused before final release of game.
        
        hoverForce = ss.hoverForce;
        hoverConstant = ss.hoverConstant;
        hoverRayDistance = ss.hoverHeight;
        rollForce = ss.rollForce;
        rollRayDistance1 = ss.rollRayDistance;
        rollRayDistance2 = ss.rollRayDistance;
        pitchForce = ss.pitchForce;
        pitchRayDistance = ss.pitchRayDistance;
        /*
        mCollider = GetComponent <Collider>();
        minCollider = mCollider.bounds.extents;
        */

    }

    private void FixedUpdate()
    {
        hoverRay1 = new Ray(transform.localPosition, transform.up * -1);
        rollRay11 = new Ray(transform.localPosition, (transform.up * -1) + transform.right);
        rollRay12 = new Ray(transform.localPosition, (transform.up * -1) + (transform.right * 2));//'
        rollRay21 = new Ray(transform.localPosition, (transform.up * -1) + (transform.right * -1));
        rollRay22 = new Ray(transform.localPosition, (transform.up * -1) + (transform.right * -2));//'
        
        pitchRay1 = new Ray(transform.localPosition, (transform.up * -1) + transform.forward);
        pitchRay2 = new Ray(transform.localPosition, (transform.up * -1) + (transform.forward * -1));
        Debug.DrawRay(transform.localPosition, transform.up * hoverRayDistance * -1, Color.green, debugRayTime, true);
        /*
        Debug.DrawRay(transform.localPosition, ((transform.up * -1) + transform.right) * rollRayDistance1, Color.blue, debugRayTime, true);
        Debug.DrawRay(transform.localPosition, ((transform.up * -1) + (transform.right * 2)) * rollRayDistance2, Color.blue, debugRayTime, true);
        Debug.DrawRay(transform.localPosition, ((transform.up * -1) + (transform.right * -1)) * rollRayDistance1, Color.blue, debugRayTime, true);
        Debug.DrawRay(transform.localPosition, ((transform.up * -1) + (transform.right * -2)) * rollRayDistance2, Color.blue, debugRayTime, true);
        */
        Debug.DrawRay(rollRay11.origin, rollRay11.direction * rollRayDistance2, Color.blue, debugRayTime, true);
        Debug.DrawRay(rollRay12.origin, rollRay12.direction * rollRayDistance2, Color.blue, debugRayTime, true);
        Debug.DrawRay(rollRay21.origin, rollRay21.direction * rollRayDistance2, Color.blue, debugRayTime, true);
        Debug.DrawRay(rollRay22.origin, rollRay22.direction * rollRayDistance2, Color.blue, debugRayTime, true);

        Debug.DrawRay(transform.localPosition, (transform.up + transform.forward) * pitchRayDistance * -1, Color.red, debugRayTime, true);
        Debug.DrawRay(transform.localPosition, (transform.up + (transform.forward * -1)) * pitchRayDistance * -1, Color.red, debugRayTime, true);

        if (Physics.Raycast(hoverRay1, out hoverHitInfo, hoverRayDistance))
        {
            vehicleRB.AddRelativeForce(Vector3.up * HoverSmoother(hoverRay1), ForceMode.Force);
        }

        if ((Physics.Raycast(rollRay11, out rollHit11, rollRayDistance1)) | (Physics.Raycast(rollRay21, out rollHit21, rollRayDistance1)))
        {
            rollHitInfo11 = rollHit11.distance;
            rollHitInfo21 = rollHit21.distance;
            rollDiff = rollHitInfo21 - rollHitInfo11;
            vehicleRB.AddRelativeTorque(Vector3.forward * rollDiff * rollForce * Time.fixedDeltaTime, ForceMode.Force);
            Debug.Log("rollDiff = " + rollDiff + " rollHitInfo1 = " + rollHitInfo11 + " rollHitInfo2 = " + rollHitInfo21);
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
