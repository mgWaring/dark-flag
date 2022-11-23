using UnityEngine;

public class Turret : MonoBehaviour
{
    Transform turret;
    Transform target;

    private void FixedUpdate()
    {
        TurretLook(target);
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
}
