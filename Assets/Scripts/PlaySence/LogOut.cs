using Unity.Netcode;
using UnityEngine;

public class LogOut : MonoBehaviour, IUISetActive
{
    [SerializeField] private SceneManager Scences;
    [SerializeField] private Login Login;

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            Scences.LogOut();
            Login.MessageTimer.StartObj = "Máy chủ bất ngờ đóng kết nối!";
            Login.MessageTimer.Play(3);
        }
    }
}
