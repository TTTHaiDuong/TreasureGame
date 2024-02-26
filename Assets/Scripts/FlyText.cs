using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlyText : MonoBehaviour
{
    private TextMeshProUGUI Text;
    private Vector3 LookForward;

    void Awake()
    {
        Text = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (transform.forward != LookForward) transform.forward = LookForward;
    }

    public void LookAt(Vector3 lookForward)
    {
        LookForward = lookForward;       
    }
    public void SetText(string text) => Text.text = text;
    public string GetText() => Text.text;
}
