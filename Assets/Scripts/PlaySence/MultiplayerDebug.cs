using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerDebug : MonoBehaviour
{
    public void HostClick()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void ClientClick()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void ServerClick()
    {
        NetworkManager.Singleton.StartServer();
    }
}
