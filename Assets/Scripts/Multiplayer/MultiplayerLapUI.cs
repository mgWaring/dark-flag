using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Multiplayer {
  public class MultiplayerLapUI : MonoBehaviour
  {
      public MultiplayerGameController gameController;
      TextMeshProUGUI text;
      int laps;

      void Start() {
          text = GetComponent<TextMeshProUGUI>();
          laps = gameController.lapCount;
      }

      // Update is called once per frame
      void Update()
      {
          text.text = string.Format("Lap: {0}/{1}", gameController.playerRacer.lap, laps);
      }
  }
}
