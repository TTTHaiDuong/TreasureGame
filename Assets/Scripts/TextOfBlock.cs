using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TextOfBlock : MonoBehaviour
{
    public TextMeshProUGUI Text;
    
    [HideInInspector] public Timer Timer;

    private void Awake()
    {
        Timer = gameObject.AddComponent<Timer>();
        Timer.FinishListening((obj) => Clear());
    }

    public void SetText(string text, float second = 0)
    {
        Timer.Break();
        Text.text = text;
        if (second != 0) Timer.Play(second);
    }

    public void Clear()
    {
        Timer.Break();
        Text.text = string.Empty;
    }
}
