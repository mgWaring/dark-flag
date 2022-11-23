using UnityEngine;

public class Turret : MonoBehaviour
{
    Transform turret;
    Transform target;
    bool turretEnable;

    private void FixedUpdate()
    {
        if (turretEnable)
        {
            TurretLook(target);
        }        
    }

    public void TurretLook(Transform target)
    {
        turret.LookAt(target);
    }

    public void TurretSetup(Transform turretTarget)
    {
        turret = GetComponent<Transform>();
        target = turretTarget.GetComponent<Transform>();
    }

    public void TurretEnable(bool tf)
    {
        turretEnable = tf;
    }
}
