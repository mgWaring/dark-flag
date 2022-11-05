using UnityEngine;

public class AntiGravManager : MonoBehaviour
{
    Rigidbody vehicleRB;
    Collider vehicleCL;//Not used at the moment.

    public Vector3 hoverRayYOffset = new Vector3(0.0f, -1.0f, 0.0f);//Not used at the moment.
    public float hoverRayDistance = 1.0f;

    public float hoverForce = 26.8f;
    public float hoverConstant = 2.5f;
    float hoverRegulator;
    float distanceToFloor;

    RaycastHit hitInfo;
    LayerMask hoverMask;

    Ray hoverRay1;
    Vector3 hoverRay1XZOffset = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 hoverRay1CombOffset;//Not used at the moment.



    void Start()
    {
        vehicleRB = GetComponent <Rigidbody>();
        vehicleCL = GetComponent <CapsuleCollider>();//Not used at the moment.
        hoverMask = LayerMask.GetMask("Ship");
        hoverRay1CombOffset = OffsetCombiner(hoverRay1XZOffset);//Not used at the moment.

    }

    private void FixedUpdate()
    {
        hoverRay1 = new Ray(transform.localPosition, transform.up * -1);
        //Leave these comments here please.
        //Debug.DrawRay(transform.localPosition, transform.up * 10 * -1, Color.yellow, 1, true);
        //Debug.DrawRay(vehicleCollider.transform.localPosition + offset, vehicleCollider.transform.forward, Color.red, 9999999, true);        

        if (GroundDetector(hoverRay1))
        {
            vehicleRB.AddRelativeForce(Vector3.up * HoverSmoother(hoverRay1), ForceMode.Force);
        }
        
    }
    
    bool GroundDetector(Ray inputRay)
    {
        bool groundDetected = false;

        if (Physics.Raycast(inputRay, out hitInfo, hoverRayDistance))
        {
            groundDetected = true;
            //Debug.Log("From GroundDetector() in AntiGravManager. Ray hit " + hitInfo.collider);
            //Debug.Log(hitInfo.distance);
        }

        if (Physics.Raycast(inputRay, out hitInfo, hoverRayDistance, hoverMask))
        {
            groundDetected = false;
            //Debug.Log("From GroundDetector() in AntiGravManager. " + hoverMask + " detected.");
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

    float HoverSmoother(Ray inputRay)
    {
        float newHoverForce = 0.0f;

        Physics.Raycast(inputRay, out hitInfo, hoverRayDistance);
        distanceToFloor = hitInfo.distance;
        //hoverRegulator increases as distance to the floor lowers. Can be adjusted by changing hoverConstant.
        hoverRegulator = (hoverConstant / (distanceToFloor + 1));
        newHoverForce = hoverForce * hoverRegulator;

        //Debug.Log("From HoverSmoother in AntiGravManager. newHoverForce = " + newHoverForce);
        return newHoverForce;
    }

}
