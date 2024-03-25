using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RemoteNetworkManager : MonoBehaviour
{
    private NetworkManager Manager;
    
    private void Awake()
    {
        Manager = FindObjectOfType<NetworkManager>();
    }

    public void StartServer()
    {
        Manager.StartServer();
    }

    public void StartClient()
    {
        Manager.StartClient();
    }

    public void StartHost()
    {
        Manager.StartHost();
    }
}
