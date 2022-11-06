using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotPlayerCountUI : MonoBehaviour
{
    public Selector playerSelector;
    public Selector botSelector;

    // Update is called once per frame
    void Update()
    {
        if (playerSelector.value == "1") {
            botSelector.selection = new string[] {"0", "1", "2", "3", "4", "5", "6", "7"};
            if (botSelector.value == "8") {
                botSelector.index = 7;
                botSelector.SetText();
            }
        } else if (playerSelector.value == "0") {
            botSelector.selection = new string[] {"0", "1", "2", "3", "4", "5", "6", "7", "8"};
        }
    }
}
