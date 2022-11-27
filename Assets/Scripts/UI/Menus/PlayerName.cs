using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerName : MonoBehaviour
{
    public Player player;
    public TextMeshPro text;

    void Start()
    {
        transform.SetParent(player.ship.transform);
        transform.localPosition = new Vector3(0f,2f,0f);
        text.text = player.racer.name;
        SetOff();
    }

    public void SetOn()
    {
        text.enabled = true;
    } 

    public void SetOff()
    {
        text.enabled = false;
    }
}
