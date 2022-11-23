using UnityEngine;

//Attach this to the MainCamera prefab.
public class PlayerCameraController : MonoBehaviour
{
    //Player Input.    
    public Vector3 cameraLookOffset = new Vector3(0.0f, 1.2f, 0.0f);
    public Vector3 cameraLookConstant = new Vector3(1.5f, 0.8f, 1.0f);
    Vector3 cameraMult = new(0.0f, 0.0f, 0.0f);
    Vector3 cameraLookSpeed = new(0.0f, 0.0f, 0.0f);    

    //Camera movement.
    public float positionSmoothing = 1.0f;
    public Vector3 cameraPosOffset = new Vector3(0.0f, 1.8f, -2.8f);//These should be in ss.
    Vector3 desiredPosition;
    Vector3 smoothedPosition;
    Transform target;
    bool playerCamEnabled;

    //Turret
    public bool PCCRaysOn = true;
    public Transform turretTarget;
    Ray turretRay;
    RaycastHit hitInfo;        

    private void FixedUpdate()
    {       
        if (playerCamEnabled)
        {            
            //Follows behind ship with lerp.
            desiredPosition = target.position + (target.forward * cameraPosOffset.z) + (Vector3.up * cameraPosOffset.y);           
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, positionSmoothing);
            transform.position = smoothedPosition;
            //Makes camera look at ship.
            transform.LookAt(target.position + cameraLookOffset + (cameraLookSpeed.y * transform.up) + (cameraLookSpeed.x * transform.right));

            //Ray goes where camera looks, moves empty transform to end of ray, turret looks at empty transform.
            turretRay = new Ray(transform.position, transform.forward);
            Physics.Raycast(turretRay, out hitInfo);
            turretTarget.position = hitInfo.point;
            if (PCCRaysOn)
            {
                Debug.DrawRay(turretRay.origin, hitInfo.point, Color.yellow);
            }            
        }
    }

    //Tells camera what it's target is.
    public void PCameraSetup(Transform playerTran)
    {        
        target = playerTran.GetComponent<Transform>();        
    }

    //Tells Turret.cs what it's target is.
    public Transform TurretTargetHunter()
    {
        turretTarget = turretTarget.GetComponent<Transform>();
        return turretTarget;
    }

    //Enables or disables camera.
    public void PCamEnable(bool enableDisable)
    {
        playerCamEnabled = enableDisable;
    }

    //Takes input from PlayerMovement.cs and puts it in a Vector3.
    public void CameraControl(float yInput, float xInput)
    {
        cameraMult.y = yInput;
        cameraMult.x = xInput;   
        cameraLookSpeed = new (cameraMult.x * cameraLookConstant.x, cameraMult.y * cameraLookConstant.y, cameraMult.z * cameraLookConstant.z);
    } 
}
