using UnityEngine;
using UnityEngine.EventSystems;

public class RoomPasswordField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform Message;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Message.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Message.gameObject.SetActive(false);
    }
}
