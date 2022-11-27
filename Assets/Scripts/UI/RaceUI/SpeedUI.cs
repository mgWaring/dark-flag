using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedUI : MonoBehaviour
{
    [HideInInspector] public GameObject target;

    Rigidbody rb;
    TextMeshProUGUI text;
    public Transform outerRing;
    public Transform middleRing;
    public Transform innerRing;
    public float minSpeed;
    public float maxSpeed;

    public void SetTarget(GameObject t)
    {
        target = t;
        rb = target.GetComponent<Rigidbody>();
        text = GetComponent<TextMeshProUGUI>();
    }

    void Start() {
        rb = target.GetComponent<Rigidbody>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        float percent = rb.velocity.magnitude / 90f;
        float val = percent * 1000.0f;
        text.text = string.Format("{0:N0}", val);
        float rotateSpeed = (maxSpeed - minSpeed) * percent + minSpeed;
        outerRing.Rotate(0f,0f,rotateSpeed * 0.25f * Time.deltaTime);
        middleRing.Rotate(0f,0f,rotateSpeed * 0.5f * Time.deltaTime);
        innerRing.Rotate(0f,0f,rotateSpeed * Time.deltaTime);
    }
}
