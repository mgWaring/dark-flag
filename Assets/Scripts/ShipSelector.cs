using System;
using UnityEngine;
using TMPro;
using System.Linq;
using UI.Pregame;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ShipSelector : MonoBehaviour {
    //max
    [SerializeField] private bool useModal;
    [SerializeField] private PlayerTile playerTile;
    public event Action<string> OnShipChange;

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
    private int index = 0;
    [HideInInspector] public ShipsScriptable value;
    public InputAction leftInput;
    public InputAction rightInput;
    public InputAction submitInput;
    public InputAction backInput;
    private float submitTimer = 0.25f;

    // Start is called before the first frame update
    private void Start() {
        selectableShips = selectableShipNames.Select(name => name.shipModel).ToArray();
        SetValues();
    }

    private void OnEnable() {
        leftInput.Enable();
        rightInput.Enable();
        submitInput.Enable();
        backInput.Enable();
    }

    //Required for new input system. Don't ask me why.
    private void OnDisable() {
        leftInput.Disable();
        rightInput.Disable();
        submitInput.Disable();
        backInput.Disable();
    }

    // Update is called once per frame
    private void Update() {
        if (submitTimer > 0.0f) {
            submitTimer -= Time.deltaTime;
        }

        if (rightInput.triggered) {
            GoRight();
        } else if (leftInput.triggered) {
            GoLeft();
        }

        if (submitInput.triggered && submitTimer <= 0.0f) {
            Confirm();
        } else if (backInput.triggered && confirmationModal.active) {
            confirmationModal.SetActive(false);
        }
    }

    public void Confirm() {
        if (confirmationModal) {
            if (confirmationModal.active) {
                StartGame();
            } else {
                confirmationModal.SetActive(true);
            }
        }

        BindDataToPlayer();
    }

    private void BindDataToPlayer() {
        //playerTile.set
    }

    public void StartGame() {
        CrossScene.racerInfo = generateRacerInfo();
        SceneManager.LoadScene(2);
    }

    private RacerInfo[] generateRacerInfo() {
        int racerCount = CrossScene.players + CrossScene.bots;
        RacerInfo[] racers = new RacerInfo[racerCount];

        for (int i = 0; i < racerCount; i++) {
            RacerInfo info;
            if (i == 0 && CrossScene.players == 1) {
                info = new RacerInfo(PlayerPrefs.GetString("playerName"), selectableShipNames[index], false);
            } else {
                info = new RacerInfo(selectableShipNames);
            }

            racers[i] = info;
        }

        return racers;
    }

    public void SetValues() {
        value = selectableShipNames[index];
        currentShip = selectableShips[index];
        // if anything is listening for this event let it know
        OnShipChange?.Invoke(value.shipName);
        if (shipHolder.childCount > 0) {
            Destroy(shipHolder.GetChild(0).gameObject);
        }

        GameObject ship = Instantiate(currentShip);
        ship.GetComponent<Rigidbody>().useGravity = false;
        ship.GetComponent<PlayerInput>().enabled = false;
        ship.GetComponent<MovementController>().enabled = false;
        ship.GetComponent<BotMovement>().enabled = false;
        ship.GetComponent<AntiGravManager>().enabled = false;
        ship.GetComponent<RigidbodyController>().enabled = false;
        ship.GetComponent<Racer>().enabled = false;
        ship.transform.SetParent(shipHolder);
        ship.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        ship.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        nameText.text = string.Format("Name: {0}", value.shipName);
        speedText.text = string.Format("Max Speed: {0}", value.thrustSpeed);
        handlingText.text = string.Format("Handling: {0}", value.yawSpeed);
        weightText.text = string.Format("Weight: {0}", value.mass);
        durabilityText.text = string.Format("Durability: {0}", value.armour);
    }

    public void GoLeft() {
        index -= 1;
        if (index < 0) {
            index = selectableShipNames.Length - 1;
        }

        SetValues();
    }

    public void GoRight() {
        index += 1;
        if (selectableShipNames.Length <= index) {
            index = 0;
        }

        SetValues();
    }
}