using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class Player : MonoBehaviour
{
    public Camera camera;
    [HideInInspector] public Racer racer;
    [HideInInspector] public ShipsScriptable ss;
    [HideInInspector] public MovementController mc;
    [HideInInspector] public PlayerCameraController pcc;
    [HideInInspector] public Turret trt;
    GameObject ship;

    public void Init()
    {
        ship = Instantiate(ss.shipModel);
        racer = ship.GetComponent<Racer>();
        mc = ship.GetComponentInChildren<MovementController>();
        mc.enabled = false;
        camera = GetComponentInChildren<Camera>();
        BotMovement bot = ship.GetComponent<BotMovement>();
        bot.enabled = false;
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

    //Tells PlayerCameraController what it's is and then enables it.
    public void AttachCamera() {
        camera.gameObject.GetComponent<Animator>().enabled = false;
        Transform camTransform = camera.transform;
        pcc = camTransform.gameObject.GetComponentInChildren<PlayerCameraController>();
        pcc.PCameraSetup(racer.gameObject.GetComponentInChildren<Transform>());
        pcc.PCamEnable(true);
        trt = GetComponentInChildren<Turret>();
        trt.TurretSetup(pcc.TurretTargetHunter());
    }
}
