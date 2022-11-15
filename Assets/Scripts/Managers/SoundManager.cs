using UnityEngine;
using Utils;

namespace Managers {
    public class SoundManager: LonelyMonoBehaviour<SoundManager> {
        [SerializeField] private AudioSource source;

        public AudioClip[] playerAnthems;
        public void PlayOnce(AudioClip clip) {
            source.clip = clip;
            source.Play();
        }
    }
}