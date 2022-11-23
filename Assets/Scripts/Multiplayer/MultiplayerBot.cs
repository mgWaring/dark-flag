using UnityEngine;

namespace Multiplayer {
    public class MultiplayerBot : MonoBehaviour
    {
        public Camera camera;
        [HideInInspector] public MultiplayerRacer racer;
        [HideInInspector] public ShipsScriptable ss;
        [HideInInspector] public MovementController mc;
        GameObject ship;

        public void Init() {
            ship = Instantiate(ss.multiplayerShipModel);
            camera = GetComponentInChildren<Camera>();
            racer = ship.GetComponent<MultiplayerRacer>();
            mc = ship.GetComponentInChildren<MovementController>();
            mc.enabled = false;
            PlayerMovement pm = ship.GetComponent<PlayerMovement>();
            pm.enabled = false;
        }
        
        public void AllowPlay() {
            mc.enabled = true;
        }

        public void SetPosRot(Vector3 pos, Quaternion rot) {
            var transform1 = transform;
            transform1.position = Vector3.zero;
            ship.transform.position = pos;
            ship.transform.rotation = rot;
            ship.transform.SetParent(transform1, true);
        }
        
        public void AttachCamera() {
            camera.gameObject.GetComponent<Animator>().enabled = false;
            Transform ship = racer.transform;
            Transform camTransform = camera.transform;
            camTransform.SetParent(ship);
            Vector3 position = ship.position;
            position += ship.up * 2;
            position -= ship.forward * 3.5f;
            camTransform.position = position;
            camTransform.rotation = ship.rotation;
            camTransform.Rotate(15,0,0);
        }
    }
}
