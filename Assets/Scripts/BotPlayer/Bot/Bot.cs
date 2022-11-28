using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public Camera camera;
    [HideInInspector] public Racer racer;
    [HideInInspector] public ShipsScriptable ss;
    [HideInInspector] public MovementController mc;
    GameObject ship;

    public void Init() {
        ship = Instantiate(ss.shipModel);
        camera = GetComponentInChildren<Camera>();
        racer = ship.GetComponent<Racer>();
        mc = ship.GetComponentInChildren<MovementController>();
        mc.enabled = false;
    }

    public void PlayOpening(string clipName) {
        camera.gameObject.GetComponent<Animator>().Play(clipName);
    }

    public void AllowPlay() {
        mc.enabled = true;
    }

    public void SetPosRot(Vector3 pos, Quaternion rot) {
        transform.position = Vector3.zero;
        ship.transform.position = pos;
        ship.transform.rotation = rot;
        ship.transform.SetParent(transform, true);
    }


    public void AttachCamera() {
        camera.gameObject.GetComponent<Animator>().enabled = false;
        Transform ship = racer.transform;
        Transform camTransform = camera.transform;
        camTransform.SetParent(ship);
        camTransform.position = ship.position;
        camTransform.position += ship.up * 2;
        camTransform.position -= ship.forward * 3.5f;
        camTransform.rotation = ship.rotation;
        camTransform.Rotate(15,0,0);
        //PlayerCameraController pcc = camTransform.gameObject.GetComponent<PlayerCameraController>();
        //pcc.PCameraSetup(racer.gameObject.GetComponentInChildren<Rigidbody>());
        //pcc.PCamEnable(true);
    }
}