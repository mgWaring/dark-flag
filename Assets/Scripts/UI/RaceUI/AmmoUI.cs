using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    public TextMeshProUGUI bulletCount;
    public TextMeshProUGUI bombCount;
    [HideInInspector] public GameObject target;
    FrontWeapon _frontWeapon;
    BackWeapon _backWeapon;

    void Start()
    {
        _frontWeapon = target.GetComponentInChildren<FrontWeapon>();
        _backWeapon = target.GetComponentInChildren<BackWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        bulletCount.text = $"{_frontWeapon.ammoCount}";
        bombCount.text = $"{_backWeapon.ammoCount}";
    }
}
