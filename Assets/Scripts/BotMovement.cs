using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMovement : MonoBehaviour
{
    public LayerMask wallLayer;
    public float sensorDistance;
    public float sensorTightness = 0.1f;
    public float stillMoveDistance = 20.0f;
    MovementController mc;
    Rigidbody rb;

    void Start() {
        mc = GetComponent<MovementController>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit forwardHit;
        RaycastHit leftHit;
        RaycastHit rightHit;

        Physics.Raycast(transform.position, ForwardDir(), out forwardHit, sensorDistance, wallLayer);
        Physics.Raycast(transform.position, LeftDir(), out leftHit, sensorDistance, wallLayer);
        Physics.Raycast(transform.position, RightDir(), out rightHit, sensorDistance, wallLayer);

        Debug.DrawRay(transform.position, ForwardDir() * sensorDistance, Color.red);
        Debug.DrawRay(transform.position, RightDir() * sensorDistance, Color.red);
        Debug.DrawRay(transform.position, LeftDir() * sensorDistance, Color.red);

        if (forwardHit.collider != null) {
            if (rb.velocity.magnitude > 10) {
                mc.MoveBackward();
            } 
            if (leftHit.collider != null && rightHit.collider == null) {
                mc.MoveRight();
            }
            if (leftHit.collider == null && rightHit.collider != null) {
                mc.MoveLeft();
            }
            if (leftHit.collider != null && rightHit.collider != null) {
                if (leftHit.distance > rightHit.distance) {
                    mc.MoveLeft();
                } else {
                    mc.MoveRight();
                }
            }
        } else {
            if (leftHit.collider != null && rightHit.collider == null) {
                if (leftHit.distance > stillMoveDistance) {
                    mc.MoveForward();
                }
                mc.MoveRight();
            }
            if (leftHit.collider == null && rightHit.collider != null) {
                if (rightHit.distance > stillMoveDistance) {
                    mc.MoveForward();
                }
                mc.MoveLeft();
            }
            if (leftHit.collider == null && rightHit.collider == null) {
                mc.MoveForward();
            }
        }
    }

    Vector3 RightDir() {
        return transform.forward + (transform.right * sensorTightness);
    }

    Vector3 ForwardDir() {
        return transform.forward;
    }

    Vector3 LeftDir() {
        return transform.forward - (transform.right * sensorTightness);
    }
}
