using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiplayer;

public class ShipDurability : MonoBehaviour
{
    [HideInInspector] public float hp = 100.0f;
    Rigidbody rb;
    float startHeight;
    public Material loadingMaterial;
    public GameObject explosionFab;
    public MeshRenderer shieldRenderer;
    float deathTimer = 1.25f;
    float spawnTimer = 1.5f;
    float shieldDisplayTimer = 0.25f;
    bool showingShield = false;
    enum State { Healthy, Exploding, Spawning };
    State state = State.Healthy;
    GameObject explosion;
    bool isBot;
    public AudioClip[] clips;
    ShipsScriptable ss;
    MapScriptable ms;
    Dictionary<int, Material[]> rememberedMaterials = new Dictionary<int, Material[]>();
    float heightLimit = 60.0f;
    float depthLimit = -30.0f;
    float fireDamage;
    Ray roofRay;
    float roofRayDistance;
    Vector3 roofRayOffset;
    float boostDamageRate;

    void Start() {
        ss = GetComponent<Ship>().details;
        rb = GetComponent<Rigidbody>();
        startHeight = transform.position.y;
        isBot = GetComponentInParent<Bot>() != null;
        Physics.IgnoreLayerCollision(8, 9); // Ignore collisions between ships and loading ships
        fireDamage = ss.fireDamage;
        roofRayDistance = ss.roofRayDistance;
        roofRayOffset = ss.roofRayOffset;
        boostDamageRate = ss.boostDamageRate;
    }

    private void HideShip() {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshes.Length; i++) {
            MeshRenderer mesh = meshes[i];
            mesh.enabled = false;
        }
    }

    private void ShowShip() {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshes.Length ; i++) {
            MeshRenderer mesh = meshes[i];
            mesh.enabled = true;
        }
    }

    void Update() {
        if (showingShield) {
            shieldRenderer.enabled = true;
            shieldDisplayTimer -= Time.deltaTime;
            if (shieldDisplayTimer <= 0) {
                shieldRenderer.enabled = false;
                showingShield = false;
                shieldDisplayTimer = 0.25f;
            }
        }
        if (state == State.Spawning) {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0) {
                spawnTimer = 1.5f;
                ChangeMaterialBack();
                state = State.Healthy;
                gameObject.layer = 8; // ship layer
            }
        }
        if (hp <= 0.0f) {
            if (state == State.Healthy) {
                if (isBot) {
                    GetComponent<BotMovement>().enabled = false;
                } else {
                    GetComponentInParent<PlayerMovement>().enabled = false;
                }
                explosion = Instantiate(explosionFab);
                explosion.transform.SetParent(transform);
                explosion.transform.localPosition = new Vector3(0.0f,0.0f,-1.15f);
                state = State.Exploding;
            } else if (state == State.Exploding) {
                deathTimer -= Time.deltaTime;
                if (deathTimer <= 0.0f) {
                    ShowShip();
                    hp = 100.0f;
                    Destroy(explosion);
                    Racer racer = GetComponent<Racer>();
                    Transform spawnPoint;
                    if (racer.lastCheckpoint == null) {
                        MultiplayerRacer mracer = GetComponent<MultiplayerRacer>();
                        spawnPoint = mracer.lastCheckpoint.spawnPointFor(transform.position);
                        transform.position = spawnPoint.position;
                        transform.rotation = spawnPoint.rotation;
                    } else {
                        spawnPoint = racer.lastCheckpoint.spawnPointFor(transform.position);
                        transform.position = spawnPoint.position;
                        transform.rotation = spawnPoint.rotation;
                    }
                    deathTimer = 1.25f;
                    gameObject.layer = 9; // loading layer
                    state = State.Spawning;
                    ChangeMaterial();
                    if (isBot) {
                        GetComponent<BotMovement>().enabled = true;
                    } else {
                        GetComponentInParent<PlayerMovement>().enabled = true;
                    }
                    GetComponent<MovementController>().Reset();
                } else if (deathTimer <= 1.0f) {
                    HideShip();
                }
            }
        }
        //Checks if ship is on it's roof and applies fire damage if true.
        roofRay = new Ray(transform.localPosition + (transform.up * roofRayOffset.y), transform.up);
        Debug.DrawRay(roofRay.origin, roofRay.direction * roofRayDistance, Color.green, 0.02f, true);
        if (Physics.Raycast(roofRay, roofRayDistance))
        {
            OnFire();
        }
        //Checks to see if player is above or below the kill ceiling/floor.
        YLimitCheck();
    }

    void ChangeMaterial() {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshes.Length; i++) { 
            MeshRenderer mesh = meshes[i];
            Material[] materials = mesh.materials;
            Material[] newMaterials = new Material[materials.Length];
            for (int j = 0; j < materials.Length; j++) {
                newMaterials[j] = loadingMaterial;
            }
            rememberedMaterials[i] = materials;
            mesh.materials = newMaterials;
        }
        shieldRenderer.enabled = false;
    }

    void ChangeMaterialBack() {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshes.Length; i++) { 
            MeshRenderer mesh = meshes[i];
            mesh.materials = rememberedMaterials[i];
        }
        shieldRenderer.enabled = false;
    }

    public void takeDamage(float damage) {
        float removal = damage - ss.armour;
        showingShield = true;
        if (removal > 0.0f) {
            hp = hp - removal;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Bullet") {
            float removal = other.GetComponent<Bullet>().damage - ss.armour;
            takeDamage(other.GetComponent<Bullet>().damage);
        } else if (other.tag == "Pickup") {
            Pickup pickup = other.gameObject.GetComponent<Pickup>();
            if (pickup.type == Pickup.PickupType.Health) {
                hp += pickup.value;
                if (hp >= 150f) {
                    hp = 150f;
                }
                Destroy(other.gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag != "Checkpoint") {
            float removal = collision.impulse.magnitude - ss.armour;
            showingShield = true;
            if (removal > 0.0f) {
                AudioSource source = GetComponent<AudioSource>();
                if (!source.isPlaying) {
                    var rand = new System.Random();
                    var clip = clips[rand.Next(clips.Length)];
                    source.PlayOneShot(clip, 1.0f);
                }
                hp = hp - removal;
            }
        }
    }

    void YLimitCheck()
    {
        if (rb.position.y >= heightLimit || rb.position.y <= depthLimit)
        {
            OnFire();
        }
        //Display out of bounds warning if getting really close?
    }

    void OnFire()
    {
        hp = hp - fireDamage;
        //play fire sound.
        //play fire animation.
        //etc.
    }

    public void BoostDamage(float boostInput)
    {
        float damage = (boostDamageRate * boostInput);
        hp = hp - damage;
    }
}
