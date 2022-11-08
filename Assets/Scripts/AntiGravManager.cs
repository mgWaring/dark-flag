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
    float hoverRegulator;
    float distanceToFloor;

    RaycastHit hoverHitInfo;
    LayerMask shipLayer;

    Ray hoverRay1;    
    Vector3 hoverRay1XZOffset = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 hoverRay1CombOffset;//Not used at the moment. Delete if still unused before final release of game.

    Ray rollRay1;
    Ray rollRay2;
    float rollDiff;
    public float rollRayDistance = 10.0f;//Put this in the scriptable objects when a good default is found.
    public float rollForce = 10000.0f;//Put this in the scriptable objects when a good default is found.
    RaycastHit rollHit1;
    RaycastHit rollHit2;
    float rollHitInfo1;
    float rollHitInfo2;

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

    }

    private void FixedUpdate()
    {
        hoverRay1 = new Ray(transform.localPosition, transform.up * -1);
        rollRay1 = new Ray(transform.localPosition,  (transform.up + transform.right) * -1);
        rollRay2 = new Ray(transform.localPosition, (transform.up + (transform.right * -1)) * -1);
        //Leave these comments here please. Delete if still unused before final release of game.
        //Debug.DrawRay(transform.localPosition, transform.up * 10 * -1, Color.yellow, 1, true);
        //Debug.DrawRay(vehicleCollider.transform.localPosition + offset, vehicleCollider.transform.forward, Color.red, 9999999, true);
        Debug.DrawRay(transform.localPosition, (transform.up + transform.right)* 10 * -1, Color.green, 10, true);
        Debug.DrawRay(transform.localPosition, (transform.up + (transform.right * -1)) * 10 * -1, Color.green, 10, true);

        if (GroundDetector(hoverRay1))
        {
            vehicleRB.AddRelativeForce(Vector3.up * HoverSmoother(hoverRay1), ForceMode.Force);
        }
        
        if (Physics.Raycast(rollRay1, out rollHit1, rollRayDistance))
        {
            rollHitInfo1 = rollHit1.distance;
        }
        if (Physics.Raycast(rollRay2, out rollHit2, rollRayDistance))
        {
            rollHitInfo2 = rollHit2.distance;
        }
        
        
        
        rollDiff = rollHitInfo2 - rollHitInfo1;
        vehicleRB.AddRelativeTorque(Vector3.forward * rollDiff * rollForce * Time.fixedDeltaTime, ForceMode.Force);
        Debug.Log("rollDiff = " + rollDiff + " rollHitInfo1 = " + rollHitInfo1 + " rollHitInfo2 = " + rollHitInfo2 + " hitInfo.distance = " + hoverHitInfo.distance);
        //Debug.Log("hoverRay1 = " + hoverRay1 + " rollRay1 = " + rollRay1 + " rollRay2 = " + rollRay2);





        //Debug.Log("hoverForce = " + hoverForce + "hoverConstant = " + hoverConstant + "hoverRayDistance = " + hoverRayDistance);
    }
    
    bool GroundDetector(Ray inputRay)
    {
        bool groundDetected = false;

        if (Physics.Raycast(inputRay, out hoverHitInfo, hoverRayDistance))
        {
            groundDetected = true;
            //Debug.Log("From GroundDetector() in AntiGravManager. Ray hit " + hitInfo.collider);
            //Debug.Log(hitInfo.distance);
        }
        /*
        if (Physics.Raycast(inputRay, out hitInfo, hoverRayDistance, hoverMask))
        {
            groundDetected = false;
            //Debug.Log("From GroundDetector() in AntiGravManager. " + hoverMask + " detected.");
        }
        */
        return groundDetected;
    }

    float HoverSmoother(Ray inputRay)
    {
        float newHoverForce = 0.0f;

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
