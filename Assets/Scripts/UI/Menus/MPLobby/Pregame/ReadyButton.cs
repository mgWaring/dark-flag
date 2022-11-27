using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Pregame
{
  public class ReadyButton : MonoBehaviour
  {

    public bool _ready;

    [SerializeField] private Color readyColor = Color.green;
    [SerializeField] private Color notReadyColor = Color.red;
    [SerializeField] private TMP_Text readyText;
    [SerializeField] private AudioClip toggleSound;
    bool readOnly;

    private Image _button;

    public void SetReadOnly()
    {
      readOnly = true;
    }

    public void Start()
    {
      _button = GetComponent<Image>();
    }

    public void SetReady(bool input)
    {
      _ready = input;
      UpdateDisplay();
    }

    public void Toggle()
    {
      if (readOnly)
      {
        return;
      }

      _ready = !_ready;
      UpdateDisplay();
      SpawnManager.Instance.SetPlayerReady(_ready);
    }

    private void UpdateDisplay()
    {
      _button.color = _ready ? readyColor : notReadyColor;
      readyText.text = _ready ? "Ready" : "Not Ready";
      SoundManager.Instance.PlayOnce(toggleSound);
    }

  }
}