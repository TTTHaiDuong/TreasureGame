using Unity.Netcode;
using UnityEngine;

public class LogOut : MonoBehaviour
{
    [SerializeField] private SceneManager Scences;

    private void Update()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost)
            Scences.LogOut();
    }
}
