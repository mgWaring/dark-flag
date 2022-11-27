using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DurabilityUI : MonoBehaviour
{
    [HideInInspector] public GameObject target;
    ShipDurability durability;
    TextMeshProUGUI text;
    public Image rightFill;
    public Image leftFill;

    // Start is called before the first frame update
    void Start()
    {
        durability = target.GetComponent<ShipDurability>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        float val = durability.hp;
        text.text = string.Format("{0:N0}", val);
        rightFill.fillAmount = durability.hp / 100f;
        leftFill.fillAmount = durability.hp / 100f;
    }
}
