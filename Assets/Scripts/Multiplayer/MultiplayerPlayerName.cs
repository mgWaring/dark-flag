using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Multiplayer
{
    public class MultiplayerPlayerName : MonoBehaviour
    {
        public MultiPlayer player;
        public TextMeshPro text;
        bool haveSetParent;

        void Start()
        {
            SetOff();
        }

        void Update()
        {
            TrySetParent();
        }

        public void SetOn()
        {
            text.enabled = true;
        } 

        public void SetOff()
        {
            text.enabled = false;
        }

        void TrySetParent()
        {
            if (!haveSetParent) {
                if (player.ship != null && player.racer != null)  {
                    transform.SetParent(player.ship.transform);
                    transform.localPosition = new Vector3(0f,2f,0f);
                    text.text = player.racer.name;
                    haveSetParent = true;
                }
            }
        }
    }
}
