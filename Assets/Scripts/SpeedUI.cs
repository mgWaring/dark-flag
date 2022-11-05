using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedUI : MonoBehaviour
{
    [HideInInspector] public GameObject target;

    Rigidbody rb;
    TextMeshProUGUI text;

    void Start() {
        rb = target.GetComponent<Rigidbody>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        float val = rb.velocity.magnitude / 90.0f * 1000.0f;
        text.text = string.Format("{0:N0} mph", val);
    }
}
