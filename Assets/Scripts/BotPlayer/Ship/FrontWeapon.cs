using Unity.VisualScripting;
using UnityEngine;

public class FrontWeapon : MonoBehaviour
{
    //Bullet firing.
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public int ammoCount;
    public float firingDelay;
    public GameObject bulletSpawner;
    float currentFiringDelay;
    
    //Front Weapon Audio.
    AudioSource gunSoundSource;
    public AudioClip gunSound;
    public float minVolume = 0.1f;
    public float maxVolume = 0.3f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.2f;

    void Start()
    {
        currentFiringDelay = firingDelay;
        gunSoundSource = bulletSpawner.GetComponent<AudioSource>();
        gunSound = gunSound.GetComponent<AudioClip>();
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
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 80f, ForceMode.Impulse);
            gunSoundSource.pitch = PitchShifter();
            gunSoundSource.volume = VolumeShifter();           
            gunSoundSource.PlayOneShot(gunSound);
        }
    }

    //Randomises gunSound pitch in range.
    private float PitchShifter()
    {
        float gunPitch = Random.Range(minPitch, maxPitch);
        return gunPitch;
    }

    //Randomises gunSound volume in range.
    private float VolumeShifter()
    {
        float gunVolume = Random.Range(minVolume, maxVolume);
        return gunVolume;
    }
}
