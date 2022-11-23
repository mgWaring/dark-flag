using Managers;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
  public class MultiPlayer : NetworkBehaviour
  {
    public Camera camera;
    [HideInInspector] public MultiplayerRacer racer;
    [HideInInspector] public ShipsScriptable ss;
    [HideInInspector] public MovementController mc;
    [HideInInspector] public PlayerCameraController pcc;
    [HideInInspector] public Turret trt;
    public GameObject ship;
    public ulong clientId;

    public void Init()
    {
      if (SpawnManager.Instance.GetClientId() == 0)
      {
        ship = Instantiate(ss.multiplayerShipModel);
        ship.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        ship.GetComponent<Racer>().enabled = false;
        racer = ship.GetComponent<MultiplayerRacer>();
        mc = ship.GetComponentInChildren<MovementController>();
        mc.enabled = false;
        camera = GetComponentInChildren<Camera>();
        BotMovement bot = ship.GetComponent<BotMovement>();
        bot.enabled = false;
      }
      else
      {
        ship = transform.GetChild(1).gameObject;
        racer = ship.GetComponent<MultiplayerRacer>();
        mc = ship.GetComponentInChildren<MovementController>();
        mc.enabled = false;
        camera = GetComponentInChildren<Camera>();
        BotMovement bot = ship.GetComponentInChildren<BotMovement>();
        bot.enabled = false;
      }
    }

    public void InitWithPosition(Vector3 pos, Quaternion rot)
    {
      if (SpawnManager.Instance.GetClientId() == 0)
      {
        ship = Instantiate(ss.multiplayerShipModel);
        ship.transform.position = pos;
        ship.transform.rotation = rot;

        ship.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        ship.transform.SetParent(transform,true);
        ship.GetComponent<Racer>().enabled = false;
        racer = ship.GetComponent<MultiplayerRacer>();
        mc = ship.GetComponentInChildren<MovementController>();
        mc.enabled = false;
        camera = GetComponentInChildren<Camera>();
        BotMovement bot = ship.GetComponent<BotMovement>();
        bot.enabled = false;
      }
      else
      {
        Debug.Log("why has this been hit");
      }
    }

    public void PlayOpening(string clipName)
    {
      camera.gameObject.GetComponent<Animator>().Play(clipName);
    }

    public void AllowPlay()
    {
      mc.enabled = true;
    }

    public void AttachCamera()
    {
      camera.gameObject.GetComponent<Animator>().enabled = false;
      Transform ship = racer.transform;
      Transform camTransform = camera.transform;
      pcc = camTransform.gameObject.GetComponentInChildren<PlayerCameraController>();
      pcc.PCameraSetup(racer.gameObject.GetComponentInChildren<Transform>());
      pcc.PCamEnable(true);
      trt = GetComponentInChildren<Turret>();
      trt.TurretSetup(pcc.TurretTargetHunter());
      trt.TurretEnable(true);
      /*camTransform.SetParent(ship);
      camTransform.position = ship.position;
      camTransform.position += ship.up * 2;
      camTransform.position -= ship.forward * 3.5f;
      camTransform.rotation = ship.rotation;
      camTransform.Rotate(15, 0, 0);*/
      //PlayerCameraController pcc = camTransform.gameObject.GetComponent<PlayerCameraController>();
      //pcc.PCameraSetup(racer.gameObject.GetComponentInChildren<Rigidbody>());
      //pcc.PCamEnable(true);
    }
  }
}
