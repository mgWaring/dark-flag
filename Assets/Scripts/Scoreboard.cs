using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Scoreboard : MonoBehaviour
{
    public GameController gameController;
    public GameObject rowPrefab;

    // Start is called before the first frame update
    private void Start()
    {
        for(int i = 0; i < gameController.playerCount; i++) {
            GameObject row = Instantiate(rowPrefab);
            row.transform.SetParent(transform);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        for(int i = 0; i < gameController.playerCount; i++) {
            GameObject row = transform.GetChild(i + 1).gameObject;
            Racer racer = gameController.racers[i];
            float bestLap = gameController.BestLapFor(racer);

            row.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0}", i + 1);
            row.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = racer.name;

            if (bestLap == 0.0f) {
                row.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "N/A";
            } else {
                row.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("{0:N2}", bestLap);
            }

            if (gameController.RacerIsFinished(racer)) {
                float finish = gameController.laps[racer].Last();
                row.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = string.Format("{0:N2}", finish);
            } else {
                row.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = string.Format("DNF");
            }
        }
        
    }
}
