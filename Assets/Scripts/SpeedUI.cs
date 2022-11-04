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
        text.text = string.Format("{0:N2}", rb.velocity.magnitude);
    }
}
