using Unity.Netcode;
using UnityEngine;

/*
 *  * isClient & isOwner & isHost
 */
namespace RelaySystem.Data {
    public class DFPlayer : NetworkBehaviour {
        public string playerName = "Lizard";

        public string playerShipName;

        //need to implement a serializable version of these two
        public Color playerColour;
        public AudioClip playerAnthem;

        public Camera camera;
        [HideInInspector] public Racer racer;
        [HideInInspector] public ShipsScriptable ss;
        [HideInInspector] public MovementController mc;
        private GameObject ship;


        public GameObject Ship { get; set; }

        // a new player enters the ring
        public override void OnNetworkSpawn() {
            // tell the spawn manager we exist
            Debug.LogFormat("I HAVE AWOKEN, and my id is: {0}", OwnerClientId);
            if (IsClient) Debug.Log("I'm a client DFPLAYER");
            if (IsHost) Debug.Log("I'm a host DFPLAYER");
            if (IsServer) Debug.Log("I'm a server DFPLAYER");
            if (IsClient && IsOwner) Debug.Log("I'm your local DFPlayer bebe");
            DontDestroyOnLoad(gameObject);
        }

        //just in case we want to do something to the ship
        public void ClaimShip(GameObject ship) {
            Ship = ship;
        }


        // Start is called before the first frame update
        public void Init() {
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

        public void AttachCamera() {
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