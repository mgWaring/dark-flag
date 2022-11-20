using UnityEngine;

public class FrontWeapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public int ammoCount;
    public float firingDelay;
    float currentFiringDelay;

    void Start()
    {
        currentFiringDelay = firingDelay;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Pickup") {
            Pickup pickup = other.gameObject.GetComponent<Pickup>();
            if (pickup.type == Pickup.PickupType.Ammo) {
                ammoCount += (int)pickup.value;
                Destroy(other.gameObject);
            }
        }
    }
    
    public void ShootGun(float input)
    {
        currentFiringDelay -= Time.deltaTime;
        if (input > 0 && currentFiringDelay <= 0 && ammoCount > 0)
        {
            ammoCount--;
            currentFiringDelay = firingDelay;
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = bulletSpawn.position;
            bullet.transform.rotation = bulletSpawn.rotation;
            bullet.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 40f, ForceMode.Impulse);
        }
    }
}
