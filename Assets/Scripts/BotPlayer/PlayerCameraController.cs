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
    float cameraFlip = 1.0f;
    Transform target;
    bool playerCamEnabled;
    Vector3 cameraRotation;

    private void FixedUpdate()
    {       
        if (playerCamEnabled)
        {            
            //Follows behind ship with lerp.
            desiredPosition = target.position + (target.forward * cameraPosOffset.z * cameraFlip) + (Vector3.up * cameraPosOffset.y);           
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, positionSmoothing);
            transform.position = smoothedPosition;
            //Makes camera look at ship.
            cameraRotation = cameraLookOffset + (cameraLookSpeed.y * transform.up) + (cameraLookSpeed.x * transform.right);
            transform.LookAt(target.position + cameraRotation);         
        }
    }

    //Tells camera what it's target is.
    public void PCameraSetup(Transform playerTran)
    {        
        target = playerTran.GetComponent<Transform>();        
    }

    //Enables or disables camera.
    public void PCamEnable(bool enableDisable)
    {
        playerCamEnabled = enableDisable;
    }

    //Takes input from PlayerMovement.cs and puts it in a Vector3.
    public void CameraControl(float yInput, float xInput, float cFlip)
    {
        cameraMult.y = yInput;
        cameraMult.x = xInput;
        cameraFlip = cFlip;
        cameraLookSpeed = new (cameraMult.x * cameraLookConstant.x, cameraMult.y * cameraLookConstant.y, cameraMult.z * cameraLookConstant.z);
    }
}
