using System;
using TMPro;
using UnityEngine;

public class TimeCountDown : MonoBehaviour, IUISetActive
{
    [SerializeField] private TextMeshProUGUI Text;
    public static Timer Timer;

    private void Awake()
    {
        if (Timer == null) Timer = new GameObject().AddComponent<Timer>();
        Timer.TickListening(SetText);
    }

    public void Init()
    {
        Awake();
    }

    private void SetText(object obj)
    {
        TimeSpan time = TimeSpan.FromSeconds(Timer.Time);

        if (time.TotalSeconds <= 5)
            Text.text = $"<color=red>{time.Minutes.ToString("D2")}:{time.Seconds.ToString("D2")}</color>";
        else if (time.TotalSeconds < 3600)
            Text.text = $"{time.Minutes.ToString("D2")}:{time.Seconds.ToString("D2")}";
        else
            Text.text = $"{time.Hours.ToString("D2")}:{time.Minutes.ToString("D2")}:{time.Seconds.ToString("D2")}";
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
