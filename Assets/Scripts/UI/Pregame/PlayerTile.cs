using TMPro;
using UnityEngine;

namespace UI.Pregame {
    public class PlayerTile: MonoBehaviour{
        public ulong pid;
        public string pname;
        public TMP_Text nameText;

        void Update() {
            nameText.text = pname;
        }
    }
}