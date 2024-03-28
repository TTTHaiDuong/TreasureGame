using GameUI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInventory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform Text;
    [SerializeField] private TextMeshProUGUI TimerRemaining;
    public Timer Timer;

    public float TimeRecover;

    public event Action Do;

    private void Awake()
    {
        Timer = gameObject.AddComponent<Timer>();
        Timer.TickListening(Count);
    }

    public void Use(bool active)
    {
        if (active && !Timer.IsRunning && transform.parent.gameObject.activeSelf)
        {
            Timer.Play(TimeRecover);
            Do?.Invoke();
        }
    }

    private void Count(object obj)
    {
        TimerRemaining.text = Timer.Time.ToString();
        if (Timer.Time == 0) TimerRemaining.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Text.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Text.gameObject.SetActive(false);
    }
}
