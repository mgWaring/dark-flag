using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Multiplayer {
    public class InGameMenu : MonoBehaviour
    {
        private GameObject child;
        public InputAction menuInput;
        public GameObject firstSelected;

        public void Exit()
        {
            SceneManager.LoadScene(0);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(2);
        }

        private void OnEnable()
        {
            menuInput.Enable();
        }

        private void OnDisable()
        {
            menuInput.Disable();
        }

        private void Start() {
            child = transform.GetChild(0).gameObject;
        }

        private void Update() {
            if (menuInput.triggered) {
                child.SetActive(!child.activeSelf);
                firstSelected.GetComponent<Selectable>().Select();
            }
        }
    }
}
