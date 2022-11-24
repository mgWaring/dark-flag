using System;
using System.Linq;
using TMPro;
using UI.Pregame;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Multiplayer {
  public class MultiplayerShipSelector : MonoBehaviour
  {
    //max
    [SerializeField] private bool useModal;
    [SerializeField] private PlayerTile playerTile;
    public event Action<int> OnShipChange;

    //andrew
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI handlingText;
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI durabilityText;
    public ShipsScriptable[] selectableShipNames;
    public Transform shipHolder;
    public GameObject confirmationModal;
    private GameObject[] selectableShips;
    private GameObject currentShip;
    [HideInInspector] public int index = 0;
    [HideInInspector] public ShipsScriptable value;
    public InputAction leftInput;
    public InputAction rightInput;
    public InputAction submitInput;
    public InputAction backInput;
    private float submitTimer = 0.25f;
    [HideInInspector] public bool readOnly = false;

    // Start is called before the first frame update
    private void Start()
    {
      selectableShips = selectableShipNames.Select(name => name.displayShipModel).ToArray();
      DisplayValues();
    }

    private void OnEnable()
    {
      leftInput.Enable();
      rightInput.Enable();
      submitInput.Enable();
      backInput.Enable();
    }

    //Required for new input system. Don't ask me why.
    private void OnDisable()
    {
      leftInput.Disable();
      rightInput.Disable();
      submitInput.Disable();
      backInput.Disable();
    }

    // Update is called once per frame
    private void Update()
    {
      if (submitTimer > 0.0f)
      {
        submitTimer -= Time.deltaTime;
      }

      if (rightInput.triggered)
      {
        GoRight();
      }
      else if (leftInput.triggered)
      {
        GoLeft();
      }

      if (submitInput.triggered && submitTimer <= 0.0f)
      {
        Confirm();
      }
      else if (backInput.triggered && confirmationModal.active)
      {
        confirmationModal.SetActive(false);
      }
    }

    public void Confirm()
    {
      if (confirmationModal)
      {
        if (confirmationModal.active)
        {
          StartGame();
        }
        else
        {
          confirmationModal.SetActive(true);
        }
      }

      BindDataToPlayer();
    }

    private void BindDataToPlayer()
    {
      //playerTile.set
    }

    public void StartGame()
    {
      CrossScene.racerInfo = generateRacerInfo();
      SceneManager.LoadScene(2);
    }

    private RacerInfo[] generateRacerInfo()
    {
      int racerCount = CrossScene.players + CrossScene.bots;
      RacerInfo[] racers = new RacerInfo[racerCount];

      for (int i = 0; i < racerCount; i++)
      {
        RacerInfo info;
        if (i == 0 && CrossScene.players == 1)
        {
          info = new RacerInfo(PlayerPrefs.GetString("playerName"), selectableShipNames[index], false);
        }
        else
        {
          info = new RacerInfo(selectableShipNames);
        }

        racers[i] = info;
      }

      return racers;
    }

    public void SetValues()
    {
      value = selectableShipNames[index];
      currentShip = selectableShips[index];
      // if anything is listening for this event let it know
      OnShipChange?.Invoke(index);
      if (shipHolder.childCount > 0)
      {
        for (int i = 0; i < shipHolder.childCount; i++)
        {
          Destroy(shipHolder.GetChild(i).gameObject);
        }
      }

      GameObject ship = Instantiate(currentShip);
      ship.transform.SetParent(shipHolder);
      ship.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
      ship.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

      nameText.text = string.Format("Name: {0}", value.shipName);
      speedText.text = string.Format("Max Speed: {0}", value.thrustSpeed);
      handlingText.text = string.Format("Handling: {0}", value.yawSpeed);
      weightText.text = string.Format("Weight: {0}", value.mass);
      durabilityText.text = string.Format("Durability: {0}", value.armour);
    }

    public void DisplayValues()
    {
      value = selectableShipNames[index];
      currentShip = selectableShips[index];
      if (shipHolder.childCount > 0)
      {
        Destroy(shipHolder.GetChild(0).gameObject);
      }

      GameObject ship = Instantiate(currentShip);
      ship.transform.SetParent(shipHolder);
      ship.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
      ship.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

      nameText.text = string.Format("Name: {0}", value.shipName);
      speedText.text = string.Format("Max Speed: {0}", value.thrustSpeed);
      handlingText.text = string.Format("Handling: {0}", value.yawSpeed);
      weightText.text = string.Format("Weight: {0}", value.mass);
      durabilityText.text = string.Format("Durability: {0}", value.armour);
    }

    public void SetReadOnly()
    {
      readOnly = true;
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
        index = selectableShipNames.Length - 1;
      }

      SetValues();
    }

    public void GoRight()
    {
      if (readOnly)
      {
        return;
      }
      index += 1;
      if (selectableShipNames.Length <= index)
      {
        index = 0;
      }

      SetValues();
    }
  }
}
