using UnityEngine;

//Attach this script to vehicle with Rigidbody.
public class RigidbodyController : MonoBehaviour
{
    Rigidbody rb;
    [HideInInspector]public ShipsScriptable ss;

    //Sets rigidbody values.
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = ss.mass;
        rb.drag = ss.drag;
        rb.angularDrag = ss.angularDrag;
    }
}
