using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RacerAgent : Agent
{
    [SerializeField] private MovementController mc;
    [SerializeField] private Racer racer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask floorLayer;

    Vector3 startPos;
    Quaternion startRot;
    Checkpoint firstCheck;
    Checkpoint lastCheck;
    GameController gc;
    float deathTimer = 5.0f;
    float distanceToFloor;

    void Start() {
        startPos = transform.position;
        startRot = transform.rotation;
        firstCheck = racer.nextCheckpoint;
        lastCheck = racer.lastCheckpoint;
        gc = GameObject.Find("/GameController").GetComponent<GameController>();
    }

    public override void OnEpisodeBegin() {
        transform.position = startPos;
        transform.rotation = startRot;
        mc.Reset();
        gc.Reset();
        racer.lap = 0;
        racer.nextCheckpoint = firstCheck;
        racer.lastCheckpoint = lastCheck;
        deathTimer = 5.0f;
        Debug.Log("Episode Starting");
    }

    public override void OnActionReceived(ActionBuffers actions) {
        float thrust = actions.ContinuousActions[0];
        float yaw = actions.ContinuousActions[1];
        mc.ThrustController(thrust);
        mc.YawController(yaw);
    }

    void Update() {
        distanceToFloor = distanceFor(transform.up * -1, floorLayer);

        if (distanceToFloor >= 20.0f) {
            deathTimer -= Time.deltaTime;
        } else {
            deathTimer = 5.0f;
        }

        if (deathTimer <= 0) {
            AddReward(-100.0f);
            EndEpisode();
        }
    }


    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(distanceFor(transform.forward, wallLayer));
        //sensor.AddObservation(distanceFor(transform.right, wallLayer));
        //sensor.AddObservation(distanceFor(transform.right * -1, wallLayer));
        sensor.AddObservation(distanceFor(transform.forward + transform.right, wallLayer));
        sensor.AddObservation(distanceFor(transform.forward + (transform.right * -1), wallLayer));
        sensor.AddObservation(distanceFor(transform.up * -1, floorLayer));

        //sensor.AddObservation(distanceFor(transform.up * -1, floorLayer));
        //sensor.AddObservation(distanceFor(transform.up * -1 + transform.forward, floorLayer));
        //sensor.AddObservation(distanceFor(transform.up * -1 - transform.forward, floorLayer));
        //sensor.AddObservation(distanceFor(transform.up * -1 + transform.right, floorLayer));
        //sensor.AddObservation(distanceFor(transform.up * -1 - transform.right, floorLayer));
    }

    private float distanceFor(Vector3 direction, LayerMask mask) {
        float distance = 20.0f;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction.normalized, out hit, 20.0f, mask)) {
            distance = hit.distance;
        }
        return distance;
    }

    public void Exploded() {
        AddReward(-50.0f);
    }

    public void AddCheckpoint(int id) {
        Debug.Log("Hit a checkpoint");
        AddReward(((float)id + 1.0f) * 10f);
        if (id == 0) {
            Debug.Log(string.Format("Hit first checkpoint: Laps now {0}", racer.lap));
            if (racer.lap == 1) {
                Debug.Log("ending cause hitpoint checked");
                AddReward(100.0f);
                EndEpisode();
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag != "Checkpoint") {
            AddReward(-1f);
        }
    }
}
