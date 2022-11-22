using UnityEngine;

//This doesn't do anything right now.
namespace Multiplayer {
    public class ShipAudioController : MonoBehaviour
    {
        public AudioSource engineSource;
        public AudioSource collisionSource;
        public GameObject targetShip;

        private void Start()
        {
            //I have no idea what I am doing.
            engineSource = GetComponent<AudioSource>();
            engineSource.Play();

        }


    }
}
