using TMPro;
using UnityEngine;

namespace Multiplayer {
    public class MainMenuSettings : MonoBehaviour
    {
        public TMP_InputField inputField;

        private void OnEnable()
        {
            string playerName = PlayerPrefs.GetString("playerName");
            if (playerName != "") {
                inputField.text = playerName;
            };
        }

        public void SaveSettings() {
            PlayerPrefs.SetString("playerName", inputField.text);
            PlayerPrefs.Save();
        }
    }
}
