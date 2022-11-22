using TMPro;
using UnityEngine;

namespace Multiplayer {
  public class SpeedUI : MonoBehaviour
  {
    [HideInInspector] public GameObject target;

    private Rigidbody rb;
    private TextMeshProUGUI text;

    private void Start()
    {
      rb = target.GetComponent<Rigidbody>();
      text = GetComponent<TextMeshProUGUI>();
    }

    public void SetTarget(GameObject t)
    {
      target = t;
      rb = target.GetComponent<Rigidbody>();
      text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    private void Update()
    {
      float val = rb.velocity.magnitude / 90.0f * 1000.0f;
      text.text = string.Format("{0:N0} mph", val);
    }
  }
}
