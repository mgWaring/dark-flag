using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDurability : MonoBehaviour
{
    [HideInInspector] public float hp = 100.0f;
    Rigidbody rb;
    float startHeight;
    public GameObject explosionFab;
    float deathTimer = 1.25f;
    enum State { Healthy, Exploding };
    State state = State.Healthy;
    GameObject explosion;
    bool isBot;
    public AudioClip[] clips;
    [HideInInspector] public ShipsScriptable ss;

    void Start() {
        rb = GetComponent<Rigidbody>();
        startHeight = transform.position.y;
        isBot = GetComponent<BotMovement>().enabled;
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
                    Vector3 checkPos = racer.lastCheckpoint.transform.position; 
                    transform.position = new Vector3(checkPos.x, startHeight, checkPos.z);
                    transform.LookAt(racer.nextCheckpoint.transform.position);
                    deathTimer = 1.25f;
                    state = State.Healthy;
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

    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag != "Checkpoint") {
            float removal = collision.impulse.magnitude - ss.armour;
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
