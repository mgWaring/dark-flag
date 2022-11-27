using UnityEngine;

//Attach this to the turret prefab.
public class Turret : MonoBehaviour
{
    //Transforms.
    public Transform meshHolder;
    public Transform target;
    Transform turret;
    
    //Turret and target movement.
    public Vector3 targetOffset = new(0.0f, 0.0f, 0.0f);//should be in ss.
    public Vector3 turretLookRange = new(1.5f, 0.0f, 0.0f);
    Vector3 turretMult;    
    Vector3 turretSwivelAmount;    
    Vector3 turretSwivel;
    public float targetSmoothing = 0.75f;
    float cameraFlip;

    bool turretEnable;

    private void FixedUpdate()
    {
        if (turretEnable)
        {
            TurretLook();
        }        
    }

    //Tells target where to be and turret where to look. Turret will look at target + player's camera input.
    private void TurretLook()
    {
        turretSwivel = Vector3.zero + (turretSwivelAmount.x * transform.right * cameraFlip);
        target.position = Vector3.Lerp(target.position, transform.position + (targetOffset.z * transform.forward * cameraFlip), targetSmoothing);
        turret.LookAt(target.position + turretSwivel);
    }

    //Assigns transforms.
    public void TurretSetup()
    {
        turret = meshHolder.GetComponent<Transform>();
        target = target.GetComponent<Transform>();
    }

    //Player input.
    public void TurretControl(float xInput, float cFlip)
    {
        turretMult.x = xInput;
        cameraFlip = cFlip;
        turretSwivelAmount = new(turretMult.x * turretLookRange.x, turretMult.y * turretLookRange.y, turretMult.z * turretLookRange.z);
    }

    public void TurretEnable(bool tf)
    {
        turretEnable = tf;
    }
}
