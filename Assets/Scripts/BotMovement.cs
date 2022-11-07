using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMovement : MonoBehaviour
{
    float botSpeedMult;
    public LayerMask wallLayer;
    public float sensorDistance;
    MovementController mc;
    ShipProfiles sb;
    Rigidbody rb;
    Racer racer;

    void Start() {
        mc = GetComponent<MovementController>();
        sb = GetComponent<ShipProfiles>();
        rb = GetComponent<Rigidbody>();
        racer = GetComponent<Racer>();

        botSpeedMult = sb.ProfileHunter("testShip", "bot_speed_modifier");
    }

    void Update()
    {
        LimitVelocity();
        TurnIfWrongWay();
        float rot = DetectDirection();
        transform.Rotate(0.0f, rot * Time.deltaTime * 20, 0.0f);
        if (rot > 60) {
            transform.Rotate(0.0f, rot, 0.0f);
        } else {
            transform.Rotate(0.0f, rot * Time.deltaTime * 20, 0.0f);
        }
        if (rot < 20) {
            mc.ThrustController(botSpeedMult);
        }
    }

    void LimitVelocity() {
        if (rb.velocity.magnitude > 45) {
            rb.velocity *= 0.7f;
        }
    }

    void TurnIfWrongWay() {
        Vector3 target = racer.nextCheckpoint.transform.position;
        target.y = transform.position.y;
        float shipDist = Vector3.Distance(transform.position, target);
        float forwardDist = Vector3.Distance(transform.position + (transform.forward * 0.1f), target);
        if (forwardDist > shipDist) {
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    } 

    float DetectDirection() {
        RaycastHit hit;
        float buffer = 0.0f;
        if (Physics.Raycast(transform.position, transform.right.normalized, out hit, 2.5f, wallLayer)) {
            rb.AddRelativeForce(Vector3.right * -1f, ForceMode.Impulse);
            buffer += 10.0f;
        }
        if (Physics.Raycast(transform.position, (transform.right * -1).normalized, out hit, 2.5f, wallLayer)) {
            rb.AddRelativeForce(Vector3.right * 1f, ForceMode.Impulse);
            buffer += 10.0f;
        }

        if (!Physics.Raycast(transform.position, transform.forward.normalized, out hit, sensorDistance, wallLayer)) {
            Debug.DrawRay(transform.position, transform.forward * sensorDistance, Color.green);
            return 0.0f;
        }

        for (float i = 0; i <= 10; i += 1.0f) {
            Vector3 ldir = (transform.forward + ((i/10.0f) * transform.right)).normalized;
            Vector3 rdir = (transform.forward - ((i/10.0f) * transform.right)).normalized;
            if (!Physics.Raycast(transform.position, ldir, out hit, sensorDistance, wallLayer)) {
                Debug.DrawRay(transform.position, ldir * sensorDistance, Color.green);
                return i * 4.5f + 7.5f + buffer;
            } else {
                Debug.DrawRay(transform.position, ldir * sensorDistance, Color.red);
            }
            if (!Physics.Raycast(transform.position, rdir, out hit, sensorDistance, wallLayer)) {
                Debug.DrawRay(transform.position, rdir * sensorDistance, Color.green);
                return i * - 4.5f - 7.5f - buffer;
            } else {
                Debug.DrawRay(transform.position, rdir * sensorDistance, Color.red);
            }
        }

        for (float i = 10.0f; i >= 0; i -= 1.0f) {
            Vector3 ldir = (((i/10.0f) * transform.forward) - transform.right).normalized;
            Vector3 rdir = (((i/10.0f) * transform.forward) + transform.right).normalized;
            if (!Physics.Raycast(transform.position, ldir, out hit, sensorDistance, wallLayer)) {
                Debug.DrawRay(transform.position, ldir * sensorDistance, Color.green);
                return 45 + (i * 4.5f);
            } else {
                Debug.DrawRay(transform.position, ldir * sensorDistance, Color.red);
            }
            if (!Physics.Raycast(transform.position, rdir, out hit, sensorDistance, wallLayer)) {
                Debug.DrawRay(transform.position, rdir * sensorDistance, Color.green);
                return -45 - (i * 4.5f);
            } else {
                Debug.DrawRay(transform.position, rdir * sensorDistance, Color.red);
            }
        }

        for (float i = 10.0f; i >= 0; i -= 1.0f) {
            Vector3 ldir = (((i/10.0f) * -transform.forward) - transform.right).normalized;
            Vector3 rdir = (((i/10.0f) * -transform.forward) + transform.right).normalized;
            if (!Physics.Raycast(transform.position, ldir, out hit, sensorDistance, wallLayer)) {
                Debug.DrawRay(transform.position, ldir * sensorDistance, Color.green);
                return 90 + (i * 4.5f + 5.0f);
            } else {
                Debug.DrawRay(transform.position, ldir * sensorDistance, Color.red);
            }
            if (!Physics.Raycast(transform.position, rdir, out hit, sensorDistance, wallLayer)) {
                Debug.DrawRay(transform.position, rdir * sensorDistance, Color.green);
                return -90 - (i * 4.5f + 5.0f);
            } else {
                Debug.DrawRay(transform.position, rdir * sensorDistance, Color.red);
            }
        }

        return 0.0f;
    }
}
