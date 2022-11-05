using RelaySystem;
using TMPro;
using UnityEngine;

namespace UI {
    public class ShowJoinCode : MonoBehaviour {
        private TMP_Text JoinCodeText =>
            gameObject.GetComponent<TMP_Text>();

        void Start() {
            UpdateActiveJoinCode();
        }

        private void UpdateActiveJoinCode() {
            if (null != RelayManager.Instance.CurrentRelayHostData.joinCode)
                JoinCodeText.text = RelayManager.Instance.CurrentRelayHostData.joinCode;
        }
    }
}