using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map")]
public class MapScriptable : ScriptableObject
{
    public string name;
    public GameObject prefab;
}
