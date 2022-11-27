using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Multiplayer
{
    public class MultiplayerAmmoUI : MonoBehaviour
    {
        public TextMeshProUGUI bulletCount;
        public TextMeshProUGUI bombCount;
        [HideInInspector] public GameObject target;
        MultiplayerFrontWeapon _frontWeapon;
        MultiplayerBackWeapon _backWeapon;

        void Start()
        {
            _frontWeapon = target.GetComponentInChildren<MultiplayerFrontWeapon>();
            _backWeapon = target.GetComponentInChildren<MultiplayerBackWeapon>();
        }

        // Update is called once per frame
        void Update()
        {
            bulletCount.text = $"{_frontWeapon.ammoCount}";
            bombCount.text = $"{_backWeapon.ammoCount}";
        }
    }
}
