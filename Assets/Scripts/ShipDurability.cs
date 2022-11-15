using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Dictionary<int, Material[]> rememberedMaterials = new Dictionary<int, Material[]>();

    void Start() {
        ss = GetComponent<Ship>().details;
        rb = GetComponent<Rigidbody>();
        startHeight = transform.position.y;
        isBot = GetComponent<BotMovement>().enabled;
        Physics.IgnoreLayerCollision(8, 9); // Ignore collisions between ships and loading ships
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
                    GetComponent<PlayerMovement>().enabled = false;
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
                    Transform spawnPoint = racer.lastCheckpoint.spawnPointFor(transform.position);
                    transform.position = spawnPoint.position;
                    transform.rotation = spawnPoint.rotation;
                    deathTimer = 1.25f;
                    gameObject.layer = 9; // loading layer
                    state = State.Spawning;
                    ChangeMaterial();
                    if (isBot) {
                        GetComponent<BotMovement>().enabled = true;
                    } else {
                        GetComponent<PlayerMovement>().enabled = true;
                    }
                    GetComponent<MovementController>().Reset();
                } else if (deathTimer <= 1.0f) {
                    HideShip();
                }
            }
        }
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
}
