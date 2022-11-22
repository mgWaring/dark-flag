using UnityEngine;

namespace Multiplayer {
    public class RandomSoundClip : MonoBehaviour
    {
        public AudioClip[] clips;
        // Start is called before the first frame update
        private void Start()
        {
            var rand = new System.Random();
            AudioSource source = GetComponent<AudioSource>();
            var clip = clips[rand.Next(clips.Length)];
            source.PlayOneShot(clip, 1.0f);
        }
    }
}
