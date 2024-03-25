using TMPro;
using UnityEngine;

public class PlayerCount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Count;

    public void SetCount(int count)
    {
        Count.text = $"Hiện tại có {count} người tham gia";
    }
}
