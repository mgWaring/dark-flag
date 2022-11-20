using UnityEngine;

//Attach this script to vehicle with Rigidbody.
public class RigidbodyController : MonoBehaviour
{
    private Rigidbody rb;
    private ShipsScriptable ss;

    //Sets rigidbody values.
    private void Start()
    {
        ss = GetComponent<Ship>().details;
        rb = GetComponent<Rigidbody>();
        rb.mass = ss.mass;
        rb.drag = ss.drag;
        rb.angularDrag = ss.angularDrag;
    }
}
