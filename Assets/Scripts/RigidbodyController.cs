using UnityEngine;

//Attach this script to vehicle with Rigidbody.
public class RigidbodyController : MonoBehaviour
{
    Rigidbody rb;
    ShipProfiles sp;

    //Sets rigidbody values.
    private void Start()
    {
        sp = GetComponent<ShipProfiles>();

        rb = GetComponent<Rigidbody>();
        rb.mass = sp.ProfileHunter("testShip", "mass");
        rb.drag = sp.ProfileHunter("testShip", "drag");
        rb.angularDrag = sp.ProfileHunter("testShip", "angular_drag");
    }
}
