using UnityEngine;

namespace Utils {
    public class LonelyMonoBehaviour<T> : MonoBehaviour where T : Component {

        private static T _instance;
        public static T Instance {
            get {
                if (_instance == null) _instance = FindObjectOfType<T>();
                if (_instance != null) return _instance;
                var gob = new GameObject {
                    name = typeof(T).Name
                };
                _instance = gob.AddComponent<T>();
                return _instance;
            }
        }

        private void Awake() {
            if (_instance == null) {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                return;
            }
            //else
            Destroy(gameObject);
        }
    }
}