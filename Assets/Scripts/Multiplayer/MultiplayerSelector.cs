using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Managers;

namespace Multiplayer
{
  public class MultiplayerSelector : Selectable
  {
    public string title;
    public string[] selection;
    [HideInInspector] public string value;
    [HideInInspector] public int index = 0;
    private TextMeshProUGUI text;
    public string type;
    bool readOnly = false;

    // Start is called before the first frame update
    private new void Start()
    {
      transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0}:", title);
      text = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
      SetText();
      switch (type)
      {
        case "map":
          SpawnManager.Instance.MapUpdate += UpdateSelection;
          break;
        case "lap":
          SpawnManager.Instance.LapUpdate += UpdateSelection;
          break;
        case "bot":
          SpawnManager.Instance.BotUpdate += UpdateSelection;
          break;
      }
      if (SpawnManager.Instance.GetClientId() != 0)
      {
        readOnly = true;
      }
    }


    public override void OnMove(AxisEventData eventData)
    {
      //Assigns the move direction and the raw input vector representing the direction from the event data.
      MoveDirection moveDir = eventData.moveDir;

      if (moveDir == MoveDirection.Right)
      {
        GoRight();
      }
      else if (moveDir == MoveDirection.Left)
      {
        GoLeft();
      }
      base.OnMove(eventData);
    }

    private void UpdateSelection(int newIndex)
    {
      index = newIndex;
      SetText();
    }

    public void SetText()
    {
      value = selection[index];
      text.text = value;
    }

    public void GoLeft()
    {
      if (readOnly)
      {
        return;
      }
      index -= 1;
      if (index < 0)
      {
        index = selection.Length - 1;
      }
      SetText();
      SpawnManager.Instance.SetSelection(type, index);
    }
    public void GoRight()
    {
      if (readOnly)
      {
        return;
      }

      index += 1;
      if (selection.Length <= index)
      {
        index = 0;
      }
      SetText();
      SpawnManager.Instance.SetSelection(type, index);
    }
  }
}
