using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnBomb : MonoBehaviour, IInitOwnerComponent
{
    [SerializeField] private RectTransform OnBombPanel;
    [SerializeField] private TextMeshProUGUI CountDown;

    private Timer.TimerHandling Start;
    private Timer.TimerHandling Tick;
    private Timer.TimerHandling Finish;
    private Player Player;

    private void Awake()
    {
        Start = (obj) => SetActive(true);
        Tick = (obj) => SetText(obj);
        Finish = (obj) => SetActive(false);
    }

    public void SetOwner(Player player)
    {
        RemoveOwner(Player);
        Player = player;

        Player.LivingTimer.StartListening(Start);
        Player.LivingTimer.TickListening(Tick);
        Player.LivingTimer.FinishListening(Finish);
    }

    public void RemoveOwner(Player player)
    {
        OnBombPanel.gameObject.SetActive(false);

        if (Player != null)
        {
            Player.LivingTimer.WhenStart -= Start;
            Player.LivingTimer.Tick -= Tick;
            Player.LivingTimer.WhenFinish -= Finish;
        }
    }

    private void SetActive(bool value)
    {
        OnBombPanel.gameObject.SetActive(value);
    }

    private void SetText(object obj)
    {
        CountDown.text = $"{(string)obj}\n<size=120%><b>{Player.LivingTimer.Time:F0}</b></size>";
    }
}

public interface IInitOwnerComponent
{
    void SetOwner(Player player);
    void RemoveOwner(Player player);

    public static GameObject[] GameObjectsWithThisInterface()
    {
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
        List<GameObject> list = new();

        foreach (GameObject obj in objects)
            if (obj.GetComponent<IInitOwnerComponent>() != null)
                list.Add(obj);

        return list.ToArray();
    }
}
