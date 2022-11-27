using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    float maxLifetime = 10.0f;
    // Update is called once per frame
    void Update()
    {
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag != "Checkpoint") {
            Destroy(gameObject);
        }
    }
}
