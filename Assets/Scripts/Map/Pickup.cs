using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType { Health, Ammo };
    public PickupType type;
    public float value;
}
