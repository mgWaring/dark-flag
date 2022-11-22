using UnityEngine;

namespace Multiplayer {
    public class ConstantRotation : MonoBehaviour
    {
        public float speed;

        // Update is called once per frame
        private void Update()
        {
            transform.Rotate(0.0f, speed * Time.deltaTime, 0.0f);
        }

    }
}
