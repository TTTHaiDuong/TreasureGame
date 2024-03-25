using TreasureGame;
using UnityEngine;

public class ItemsBarInside : MonoBehaviour
{
    [SerializeField] private RectTransform ItemsBar;
    [SerializeField] private ItemInventory ItemManage;

    private Vector3 StartMousePosition;
    private Vector3 StartItemsBarPosition;
    private bool IsDragging;

    public RectTransform Rect;

    private int Count = 1;
    public float Distance;

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Dragging(Input.GetMouseButton(0));
    }

    public void Dragging(bool active)
    {
        if (active && RectTransformUtility.RectangleContainsScreenPoint(ItemsBar, Input.mousePosition))
        {
            if (!IsDragging)
            {
                StartItemsBarPosition = transform.position;
                StartMousePosition = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y);
                IsDragging = true;
            }
            float dragDelta = transform.TransformDirection(new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y) - StartMousePosition).x;
            Vector3 newPos = transform.position;
            newPos.x = StartItemsBarPosition.x + dragDelta;
            transform.position = newPos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            IsDragging = false;
        }
    }

    public void Show(ItemList list)
    {
        Count = list.Count;
    }

    public void Clear()
    {
        foreach (Transform item in transform)
            if (item.CompareTag("ItemInventory")) Destroy(item.gameObject);
    }

    public Vector2 FirstItemPoint()
    {
        foreach (Transform item in transform)
            if (item.CompareTag("ItemInventory") && item.GetComponent<ItemInventory>() is ItemInventory itemManage)
            {
                RectTransform rect = itemManage.GetComponent<RectTransform>();
                float x = rect.localPosition.x - rect.sizeDelta.x / 2 - Distance;
                float y = rect.localPosition.y - rect.sizeDelta.y / 2;
                return new Vector2(x, y) + new Vector2(Rect.localPosition.x, Rect.localPosition.y);
            }
        return new();
    }

    public Vector2 LastItemPoint()
    {
        foreach (Transform item in transform)
            if (item.CompareTag("ItemInventory") && item.GetComponent<ItemInventory>() is ItemInventory itemManage)
            {
                RectTransform rect = itemManage.GetComponent<RectTransform>();
                float x = rect.localPosition.x + rect.sizeDelta.x / 2 + Distance;
                float y = rect.localPosition.y - rect.sizeDelta.y / 2;
                return new Vector2(x, y) + new Vector2(Rect.localPosition.x, Rect.localPosition.y);
            }
        return new();
    }
}
