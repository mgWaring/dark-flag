using Managers;
using TMPro;
using UnityEngine;
using Unity.Netcode;

namespace UI {
    public class ShowJoinCode : MonoBehaviour {
        private TMP_Text JoinCodeText =>
            gameObject.GetComponent<TMP_Text>();
        bool _haveSetText = false;

        private void Update() {
          if (!_haveSetText) {
            SetText();
          }
        }

        public void CopyToClipboard()
        {
          GUIUtility.systemCopyBuffer = JoinCodeText.text;
        }

        private void SetText()
        {
            if (SpawnManager.Instance._code.Value.ToString() != "") {
              JoinCodeText.text = SpawnManager.Instance._code.Value.ToString();
              _haveSetText = true;
            }
        }
    }
}
