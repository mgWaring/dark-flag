using UnityEngine;

namespace BotPlayer.Ship {
    [RequireComponent(typeof(Rigidbody))]
    public class EngineGlow : MonoBehaviour {
        [SerializeField] private Light[] engineLights;
        [SerializeField] private float brightness = 2.5f;
        private Rigidbody rb;
        private float prevMax = 1000f;
        [SerializeField] private Renderer[] particleRenderers;
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material boostMaterial;
        private float prevBoost = 0f;

        private void Start() {
            rb = GetComponent<Rigidbody>();
        }

        public void Glow(float mag, float boost) {
            prevMax = Mathf.Max(prevMax, rb.velocity.magnitude);
            var vel = rb.velocity.magnitude / prevMax; //normalise against all time high
            var boostFactor = boost * vel * brightness; // at no boost this will == 0

            var redness = Mathf.Max(
                Mathf.Clamp(60 * mag, 0, 60),
                boostFactor * 220
            );
            var greenness = Mathf.Clamp(50 * mag, 145, 205);
            var blueness = Mathf.Clamp(50 * mag, 155, 215);
            var awesomeColour = new Color(
                redness,
                boostFactor == 0 ? greenness : greenness / 3,
                boostFactor == 0 ? blueness : blueness / 3
            );

            foreach (var source in engineLights) {
                source.color = awesomeColour;
                source.intensity = mag * brightness * vel;
            }

            if (prevBoost != boost) {
                ToggleParticleMaterials(boost);
            }
        }

        private void ToggleParticleMaterials(float boost) {
            prevBoost = boost;
            foreach (var matRenderer in particleRenderers) {
                matRenderer.material = boost > 0 ? boostMaterial : normalMaterial;
            }
        }
    }
}