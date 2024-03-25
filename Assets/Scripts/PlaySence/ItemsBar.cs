using UnityEngine;

public class ItemsBar : MonoBehaviour
{
    [SerializeField] private ItemsBarInside Inside;

    private void Update()
    {
        if (Inside.FirstItemPoint().x > LeftLimit().x && Inside.LastItemPoint().x > RightLimit().x)
        {
            Vector2 move = Inside.Rect.position;
            move.x -= 5;
            Inside.Rect.position = move;
        }

        if (Inside.LastItemPoint().x < RightLimit().x && Inside.FirstItemPoint().x < LeftLimit().x)
        {
            Vector2 move = Inside.Rect.position;
            move.x += 5;
            Inside.Rect.position = move;
        }
    }

    public Vector2 LeftLimit()
    {
        RectTransform rect = GetComponent<RectTransform>();
        return new(-rect.sizeDelta.x / 2, -rect.sizeDelta.y / 2);
    }

    public Vector2 RightLimit()
    {
        RectTransform rect = GetComponent<RectTransform>();
        return new(rect.sizeDelta.x / 2 - 30, -rect.sizeDelta.y / 2);
    }
}
