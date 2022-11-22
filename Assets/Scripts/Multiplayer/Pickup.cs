using UnityEngine;

namespace Multiplayer {
    public class Pickup : MonoBehaviour
    {
        public enum PickupType { Health, Ammo };
        public PickupType type;
        public float value;
    }
}
