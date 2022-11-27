using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map")]
public class MapScriptable : ScriptableObject
{
    public string name;
    public GameObject prefab;
    public string cameraClipName;
    public float killCeilingHeight = 140.0f;
    public float killFloorDepth = -140.0f;
    public string singleplayerScene;
    public string multiplayerScene;
}
