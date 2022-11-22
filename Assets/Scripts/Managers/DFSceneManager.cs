using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Managers {
    public class DFSceneManager: LonelyNetworkBehaviour<DFSceneManager> {
        public void TryLoadScene(string sceneName) {
            if (IsServer && !string.IsNullOrEmpty(sceneName))
            {
                var status = NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                if (status != SceneEventProgressStatus.Started)
                {
                    Debug.LogWarning($"Failed to load {sceneName} " +
                                     $"with a {nameof(SceneEventProgressStatus)}: {status}");
                }
            }
        }
    }
}