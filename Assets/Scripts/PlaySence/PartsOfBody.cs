using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsOfBody : MonoBehaviour
{
    [SerializeField] private Player Player;

    public bool IsThisChild(GameObject obj)
    {
        if (obj.transform.parent == transform) return true;
        else return false;
    }

    public void CloseToExplosion(float time, Player killer)
    {
        if (Player == null) return;
        if (!Player.LivingTimer.IsRunning)
        {
            if (killer == Player) Player.LivingTimer.TickObj = "Bạn đã bị nổ tung bởi chính bạn!";
            else Player.LivingTimer.TickObj = $"Bạn đã bị nổ tung bởi {killer.Name}!";

            Player.Exploded(time);
        }
    }

    public void SetActiveChildren(bool value)
    {
        foreach (Transform transform in transform)
            if (transform.CompareTag("PartOfBody")) transform.gameObject.SetActive(value);
    }
}
