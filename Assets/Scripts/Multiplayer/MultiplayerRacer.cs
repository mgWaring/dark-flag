using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
  public class MultiplayerRacer : NetworkBehaviour
  {
    [HideInInspector] public int id;
    [HideInInspector] public string name;
    [HideInInspector] public MultiplayerCheckpoint lastCheckpoint;
    [HideInInspector] public MultiplayerCheckpoint nextCheckpoint;
    [HideInInspector] public int lap;
    private int checkpointCount;

    public float GetPosition()
    {
      SetCheckpointCount();
      int b = lastCheckpoint.id + (lap * checkpointCount);

      if (!nextCheckpoint) return b;
      float total = Vector3.Distance(nextCheckpoint.transform.position, lastCheckpoint.transform.position);
      float current = Vector3.Distance(nextCheckpoint.transform.position, transform.position);
      float fraction = 1.0f - current / total;
      return b + fraction;
    }

    private void SetCheckpointCount()
    {
      if (lastCheckpoint != null) {
        checkpointCount = lastCheckpoint.gameObject.transform.parent.childCount;
      }
    }
  }
}
