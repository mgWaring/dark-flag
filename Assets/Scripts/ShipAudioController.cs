using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This doesn't do anything right now.
public class ShipAudioController : MonoBehaviour
{
    public AudioSource engineSource;
    public AudioSource collisionSource;
    public GameObject targetShip;

    void Start()
    {
        //I have no idea what I am doing.
        engineSource = GetComponent<AudioSource>();
        engineSource.Play();

    }


}
