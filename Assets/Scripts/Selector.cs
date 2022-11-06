using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Selector : MonoBehaviour
{
    public string title;
    public string[] selection;
    [HideInInspector] public string value;
    [HideInInspector] public int index = 0;
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0}:", title);
        text = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        SetText();
    }

    public void SetText() {
        value = selection[index];
        text.text = value;
    }

    public void GoLeft() {
        index -= 1;
        if (index < 0) {
            index = selection.Length - 1;
        }
        SetText();
    }
    public void GoRight() {
        index += 1;
        if (selection.Length <= index) {
            index = 0;
        }
        SetText();
    }
}
