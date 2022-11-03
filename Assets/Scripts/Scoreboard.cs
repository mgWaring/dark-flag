using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    public GameController gameController;
    public GameObject rowPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < gameController.playerCount; i++) {
            GameObject row = Instantiate(rowPrefab);
            row.transform.SetParent(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < gameController.playerCount; i++) {
            GameObject row = transform.GetChild(i + 1).gameObject;
            Racer racer = gameController.racers[i];
            row.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0}", i + 1);
            row.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = string.Format("{0}", racer.id);

            if (gameController.finishes.ContainsKey(racer)) {
                float finish = gameController.finishes[racer];
                row.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("{0:N2}", finish);
            } else {
                row.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("DNF");
            }
        }
        
    }
}
