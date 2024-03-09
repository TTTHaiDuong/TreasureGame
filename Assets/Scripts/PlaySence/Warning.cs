using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Warning : MonoBehaviour
{
    public TextMeshProUGUI TextGUI;

    // Start is called before the first frame update
    void Start()
    {
        TextGUI.gameObject.SetActive(false);
    }

    public void OnPointedEnter()
    {
        TextGUI.gameObject.SetActive(true);
    }

    public void OnPointedExit()
    {
        TextGUI.gameObject.SetActive(false);
    }
}
