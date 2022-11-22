using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
  public class Racer : NetworkBehaviour
  {
    [HideInInspector] public int id;
    [HideInInspector] public string name;
    [HideInInspector] public Checkpoint lastCheckpoint;
    [HideInInspector] public Checkpoint nextCheckpoint;
    [HideInInspector] public int lap = 0;
    private int checkpointCount;

    private void Start()
    {
      checkpointCount = lastCheckpoint.gameObject.transform.parent.childCount;
    }

    public float GetPosition()
    {
      int b = lastCheckpoint.id + (lap * checkpointCount);

      if (nextCheckpoint)
      {
        float total = Vector3.Distance(nextCheckpoint.transform.position, lastCheckpoint.transform.position);
        float current = Vector3.Distance(nextCheckpoint.transform.position, transform.position);
        float fraction = 1.0f - current / total;
        return (float)b + fraction;
      }
      else
      {
        return b;
      }
    }
  }
}
