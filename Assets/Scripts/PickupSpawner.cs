using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public float spawnEvery;
    public GameObject pickupPrefab;
    public float value;
    private float spawnTimer;
    private GameObject currentPickup;

    // Start is called before the first frame update
    private void Start()
    {
        spawnTimer = 0.0f;
    }

    // Update is called once per frame
    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0) {
            SpawnPickup();
            spawnTimer = spawnEvery;
        }
    }

    private void SpawnPickup() {
        if (currentPickup != null) {
            return;
        }
        currentPickup = Instantiate(pickupPrefab);
        Pickup pickup = currentPickup.GetComponent<Pickup>();
        pickup.value = value;
        pickup.transform.position = transform.position + (transform.up * 2);
    }
}
