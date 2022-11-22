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
    public GameObject ship;
    public ulong clientId;

    // Start is called before the first frame update
    public void Init()
    {
      if (SpawnManager.Instance.GetClientId() == 0)
      {
        ship = Instantiate(ss.shipModel);
        ship.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
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

    public void PlayOpening(string clipName)
    {
      camera.gameObject.GetComponent<Animator>().Play(clipName);
    }

    public void AllowPlay()
    {
      mc.enabled = true;
    }

    public void SetPosRot(Vector3 pos, Quaternion rot)
    {
      transform.position = Vector3.zero;
      ship.transform.position = pos;
      ship.transform.rotation = rot;
      ship.transform.SetParent(transform, true);
    }

    public void AttachCamera()
    {
      camera.gameObject.GetComponent<Animator>().enabled = false;
      Transform ship = racer.transform;
      Transform camTransform = camera.transform;
      camTransform.SetParent(ship);
      camTransform.position = ship.position;
      camTransform.position += ship.up * 2;
      camTransform.position -= ship.forward * 3.5f;
      camTransform.rotation = ship.rotation;
      camTransform.Rotate(15, 0, 0);
      //PlayerCameraController pcc = camTransform.gameObject.GetComponent<PlayerCameraController>();
      //pcc.PCameraSetup(racer.gameObject.GetComponentInChildren<Rigidbody>());
      //pcc.PCamEnable(true);
    }
  }
}
