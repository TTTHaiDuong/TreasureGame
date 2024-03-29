using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    private float _Value;

    public RectTransform Bar;
    public RectTransform Stretch;
    public float Maximum;
    public float Step;

    public float Value
    {
        set
        {
            if (value > Maximum) return;
            _Value = value;

            float maxLength = Bar.sizeDelta.y;

            RectTransform rectValue = Stretch.GetComponent<RectTransform>();
            Vector2 offset = Stretch.sizeDelta;
            offset.y = maxLength - (value * maxLength) / Maximum;
            rectValue.offsetMax = offset;
        }
        get => _Value;
    }
}
