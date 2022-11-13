using UnityEngine;

//Add this script to the vehicle object that has a rigidbody.
//Forces applied by the AntiGravManager are affected by the rigidbody mass.
public class AntiGravManager : MonoBehaviour
{    
    //+++++++++++++++++ITS A MESS RIGHT NOW, DONT LOOK++++++++++++++++++++
    [HideInInspector] public ShipsScriptable ss;

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
    
    float distanceToFloor;

    RaycastHit hoverHitInfo;
    //float distanceToFloor;      

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

    ShipsScriptable ss;

    void Start()
    {
        ss = GetComponent<Ship>().details;
        vehicleRB = GetComponent <Rigidbody>();

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
            //Debug.Log("rollDiff = " + rollDiff + " rollHitInfo1 = " + rollHitInfo11 + " rollHitInfo2 = " + rollHitInfo21);
        }

        if ((Physics.Raycast(pitchRay1, out pitchHit1, pitchRayDistance)) | (Physics.Raycast(pitchRay2, out pitchHit2, pitchRayDistance)))
        {
            pitchHitInfo1 = pitchHit1.distance;
            pitchHitInfo2 = pitchHit2.distance;
            pitchDiff = pitchHitInfo1 - pitchHitInfo2;
            vehicleRB.AddRelativeTorque(Vector3.right * pitchDiff * pitchForce * Time.fixedDeltaTime, ForceMode.Force);
            //Info.Debug.Log("pitchDiff = " + pitchDiff + " pitchHitInfo1 = " + pitchHitInfo1 + " pitchHitInfo2 = " + pitchHitInfo2);
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
