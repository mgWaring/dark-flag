using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Multiplayer;

/*
* todo: Extend the player tile so that it can handle input to handle:
* changing colour,
* changing anthem,
 * */

namespace UI.Pregame
{
  public class PlayerTile : MonoBehaviour
  {
    public TextMeshProUGUI nameText;
    public ReadyButton readyButton;
    public Camera camera;
    public RawImage shipImage;
    int index;

    [SerializeField] private ShipSelector _shipSelector;

    private void OnEnable()
    {
      _shipSelector.OnShipChange += SetPlayerShip;
    }

    private void OnDisable()
    {
      _shipSelector.OnShipChange -= SetPlayerShip;
    }

    private void Start()
    {
      // when the input is changed update the player object with the new name
      index = transform.GetSiblingIndex();
      transform.GetChild(0).transform.position += new Vector3(1000.0f * index, 1000.0f * index, 1000.0f * index);
      RenderTexture outputTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
      camera.targetTexture = outputTexture;
      shipImage.texture = outputTexture;
      if (SpawnManager.Instance.CurrentPlayerIndex() != index)
      {
        _shipSelector.SetReadOnly();
        readyButton.SetReadOnly();
      }

      UpdateName("New Player");
    }

    public void UpdateName(string name)
    {
      nameText.text = name;
    }

    private void SetPlayerShip(int shipIndex)
    {
      SpawnManager.Instance.SetPlayerShip(shipIndex);
    }
    
  }
}