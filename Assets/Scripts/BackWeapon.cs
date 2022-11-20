using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackWeapon : MonoBehaviour
{
    public GameObject bombPrefab;
    public Transform bombSpawn;
    public int ammoCount;
    public float firingDelay;
    public InputAction fireInput;
    private float currentFiringDelay;

    private void OnEnable()
    {
        fireInput.Enable();
    }

    private void OnDisable()
    {
        fireInput.Disable();
    }

    // Start is called before the first frame update
    private void Start()
    {
        currentFiringDelay = firingDelay;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Pickup") {
            Pickup pickup = other.gameObject.GetComponent<Pickup>();
            if (pickup.type == Pickup.PickupType.Ammo) {
                ammoCount++;
                Destroy(other.gameObject);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        currentFiringDelay -= Time.deltaTime;
        if (fireInput.ReadValue<float>() > 0 && currentFiringDelay <= 0 && ammoCount > 0) {
            ammoCount--;
            currentFiringDelay = firingDelay;
            GameObject bomb = Instantiate(bombPrefab);
            bomb.transform.position = bombSpawn.position;
            bomb.transform.rotation = bombSpawn.rotation;
            bomb.transform.Rotate(90f,0f,0f);
            bomb.GetComponent<Rigidbody>().AddForce(bomb.transform.up * 10f, ForceMode.Impulse);
        }
    }
}
