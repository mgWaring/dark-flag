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
                } else if (deathTimer <= 1.0f) {
                    HideShip();
                }
            }
        }
    }

    void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag != "Checkpoint") {
            Rigidbody collidedRB = other.gameObject.GetComponent<Rigidbody>();
            Vector3 collidedSpeed;
            if (collidedRB) {
                collidedSpeed = collidedRB.velocity;
            } else {
                collidedSpeed = new Vector3(0.0f, 0.0f, 0.0f);
            }
            float diff = (rb.velocity - collidedSpeed).magnitude;
            hp = hp - (diff * 2);
        }
    }
}
