using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBot : MonoBehaviour
{
    Racer racer;
    NavMeshAgent navMeshAgent;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        racer = GetComponent<Racer>();
    }

    // Update is called once per frame
    void Update()
    {
        var target = racer.nextCheckpoint.transform.position;
        target.z = transform.position.z;
        navMeshAgent.destination = target;
    }
}
