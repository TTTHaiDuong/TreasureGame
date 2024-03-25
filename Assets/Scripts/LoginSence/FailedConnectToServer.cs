using UnityEngine;

public class FailedConnectToServer : MonoBehaviour
{ 
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
