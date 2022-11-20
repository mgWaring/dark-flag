using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PositionUI : MonoBehaviour
{
    public GameController gameController;
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        text.text = PositionFromInt(gameController.PositionForPlayer());
    }

    private string PositionFromInt(int pos) {
        switch(pos) {
            case 0:
              return "1st";
            case 1:
              return "2nd";
            case 2:
              return "3rd";
            case 3:
              return "4th";
            case 4:
              return "5th";
            case 5:
              return "6th";
            case 6:
              return "7th";
            case 7:
              return "8th";
            default:
              return "baws";
        }
    }
}
