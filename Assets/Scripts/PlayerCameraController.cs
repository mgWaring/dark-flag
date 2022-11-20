using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public float cameraSmoothing = 0.125f;
    private Rigidbody target;
    public Vector3 cameraOffset = new Vector3(0.0f,0.0f,0.0f);
    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;
    private bool playerCamEnabled;

    private void LateUpdate()
    {
        if (playerCamEnabled)
        {
            desiredPosition = target.position + cameraOffset;
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, cameraSmoothing);
            transform.position = smoothedPosition;
        }
    }

    public void PCameraSetup(Rigidbody playerRB/*, Vector3 shipCamOffset*/)
    {        
        target = playerRB.GetComponent<Rigidbody>();
        //cameraOffset = shipCamOffset;
    }

    public void PCamEnable(bool enableDisable)
    {
        playerCamEnabled = enableDisable;
    }

}
