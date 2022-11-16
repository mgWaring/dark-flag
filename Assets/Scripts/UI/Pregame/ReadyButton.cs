﻿using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Pregame {
    public class ReadyButton : MonoBehaviour {
        
        private bool _ready;

        [SerializeField] private Color readyColor = Color.green;
        [SerializeField] private Color notReadyColor = Color.red;
        [SerializeField] private TMP_Text readyText;
        [SerializeField] private AudioClip toggleSound;
        
        private Image _button;

        public void Start() {
            _button = GetComponent<Image>();
        }
        public void Toggle() {
            _ready = !_ready;
            _button.color = _ready ? readyColor : notReadyColor;
            readyText.text = _ready ? "ready" : "not ready";
            SoundManager.Instance.PlayOnce(toggleSound);
        }
        
    }
}