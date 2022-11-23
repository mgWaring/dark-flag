using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public Turret tur;
    public float positionSmoothing = 1.0f;    
    public Vector3 cameraPosOffset = new Vector3(0.0f,1.8f,-2.8f);//These should be in ss.
    public Vector3 cameraLookOffset = new Vector3(0.0f, 1.2f, 0.0f);
    public Vector3 cameraLookConstant = new Vector3(1.5f, 0.8f, 1.0f);
    Transform target;
    
    Vector3 desiredPosition;
    Vector3 smoothedPosition;
    bool playerCamEnabled;
    Vector3 cameraMult = new(0.0f,0.0f,0.0f);
    Vector3 cameraLookSpeed = new(0.0f,0.0f,0.0f);
    Ray turretRay;
    RaycastHit hitInfo;
    public Transform turretTarget;

    private void FixedUpdate()
    {       
        if (playerCamEnabled)
        {            
            desiredPosition = target.position + (target.forward * cameraPosOffset.z) + (Vector3.up * cameraPosOffset.y);           
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, positionSmoothing);
            transform.position = smoothedPosition;
            transform.LookAt(target.position + cameraLookOffset + (cameraLookSpeed.y * transform.up) + (cameraLookSpeed.x * transform.right));

            turretRay = new Ray(transform.position, transform.forward);
            Physics.Raycast(turretRay, out hitInfo);
            turretTarget.position = hitInfo.point;
            Debug.DrawLine(turretRay.origin, hitInfo.point, Color.yellow);
        }

    }

    public void PCameraSetup(Transform playerTran)
    {        
        target = playerTran.GetComponent<Transform>();
        
    }

    public Transform TurretTargetHunter()
    {
        turretTarget = turretTarget.GetComponent<Transform>();
        return turretTarget;
    }

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
