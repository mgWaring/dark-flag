using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Checkpoint : MonoBehaviour
{
  [HideInInspector] public int id;
  [HideInInspector] public GameController gameController;
  private Transform[] spawnPoints;

  private void Start()
  {
    spawnPoints = new Transform[transform.childCount];
    for (int i = 0; i < transform.childCount; i++)
    {
      spawnPoints[i] = transform.GetChild(i);
    }
    id = transform.GetSiblingIndex();

    if (GameObject.Find("/GameController") != null)
    {
      gameController = GameObject.Find("/GameController").GetComponent<GameController>();
    }
  }

  public Transform spawnPointFor(Vector3 pos)
  {
    if (spawnPoints.Length == 0)
    {
      return transform;
    }
    else if (spawnPoints.Length == 1)
    {
      return spawnPoints[0];
    }
    else
    {
      return spawnPoints.OrderBy(t => Mathf.Abs(Vector3.Distance(pos, t.position))).ToArray()[0];
    }
  }

  private void OnTriggerEnter(Collider collider)
  {
    if (collider.tag == "Ship")
    {
      Racer racer = collider.gameObject.GetComponentInParent<Racer>();

      if (this == racer.nextCheckpoint)
      {
        Checkpoint next = NextCheckpoint();
        if (next == null)
        {
          racer.lastCheckpoint = this;
          racer.nextCheckpoint = transform.parent.GetChild(0).GetComponent<Checkpoint>();
        }
        else
        {
          racer.lastCheckpoint = this;
          racer.nextCheckpoint = next;
          if (gameController != null && id == 0)
          {
            gameController.RegisterLap(racer);
          }
        }
      }
    }
  }

  private Checkpoint NextCheckpoint()
  {
    int max = transform.parent.childCount - 1;
    if (id + 1 > max)
    {
      return null;
    }
    Transform obj = transform.parent.GetChild(id + 1);
    return obj.GetComponent<Checkpoint>();
  }
}
