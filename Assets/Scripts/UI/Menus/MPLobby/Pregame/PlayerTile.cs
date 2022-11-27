using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Multiplayer;

namespace UI.Pregame
{
  public class PlayerTile : MonoBehaviour
  {
    public TextMeshProUGUI nameText;
    public ReadyButton readyButton;
    public Camera camera;
    public RawImage shipImage;
    [HideInInspector] public int clientId = 99999;
    int index;
    bool setClientId = false;

    [SerializeField] private MultiplayerShipSelector _shipSelector;

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
      if (SpawnManager.Instance._players.Count > index) {
        clientId = SpawnManager.Instance._players[index].clientId;
        setClientId = true;
      }
      transform.GetChild(0).transform.position += new Vector3(3000.0f * (index + 1), 3000.0f * (index + 1), 3000.0f * (index + 1));
      RenderTexture outputTexture = new RenderTexture(384,115, 16, RenderTextureFormat.ARGB32);
      camera.targetTexture = outputTexture;
      shipImage.texture = outputTexture;
      if (SpawnManager.Instance.GetClientId() != clientId)
      {
        _shipSelector.SetReadOnly();
        readyButton.SetReadOnly();
      }

      UpdateName("New MultiPlayer");
    }

    public void UpdateName(string name)
    {
      nameText.text = name;
    }

    private void SetPlayerShip(int shipIndex)
    {
      SpawnManager.Instance.SetPlayerShip(shipIndex);
    }

    private void Update()
    {
      if (!setClientId && SpawnManager.Instance._players.Count > index) {
        clientId = SpawnManager.Instance._players[index].clientId;
        setClientId = true;
      }
    }
    
  }
}
