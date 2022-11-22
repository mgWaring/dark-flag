using TMPro;
using UnityEngine;

namespace Multiplayer {
    public class DurabilityUI : MonoBehaviour
    {
        [HideInInspector] public GameObject target;
        private ShipDurability durability;
        private TextMeshProUGUI text;

        // Start is called before the first frame update
        private void Start()
        {
            durability = target.GetComponent<ShipDurability>();
            text = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        private void Update()
        {
            float val = durability.hp;
            text.text = string.Format("Durability: {0:N0}%", val);
        }
    }
}
