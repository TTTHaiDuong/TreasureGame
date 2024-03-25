using GameItems;
using UnityEngine;
using UnityEngine.UI;

public class FloatingItem : MonoBehaviour
{
    private HarmonicOscillation Osc;
    private Vector3 OriginalPosition;

    [SerializeField] private ItemImage ItemImage;
    [SerializeField] private Image Image;
    [SerializeField] private RectTransform RectImage;

    public GameItem Item;

    private void Awake()
    {
        Osc = new(0.2f, 5);
        RectImage = Image.GetComponent<RectTransform>();
        OriginalPosition = RectImage.anchoredPosition;
    }

    private void Update()
    {
        Image.transform.forward = Camera.main.transform.forward;
        Oscillation();
    }

    private void Oscillation()
    {
        Vector3 pos = OriginalPosition;
        pos.y = Osc.Turn(Time.deltaTime) + OriginalPosition.y;
        RectImage.anchoredPosition = pos;
    }

    public void SetUp(ItemImage item)
    {
        Image.sprite = item.Image.sprite;
        Item = item.Item.Clone() as GameItem;
        gameObject.SetActive(true);
    }

    public void SetUp(GameItem item)
    {
        Image.sprite = ItemImage.GetSprite(item);
        Item = item.Clone() as GameItem;
        gameObject.SetActive(true);
    }

    public void Picked()
    {
        gameObject.SetActive(false);
    }
}

public class HarmonicOscillation
{
    public HarmonicOscillation(float amplitude, float angularSpeed)
    {
        Amplitude = amplitude;
        AngularSpeed = angularSpeed;
    }

    public float Amplitude;
    public float AngularSpeed;
    public float Time;
    public float Phi;

    public float Cycle { get { return 360 / AngularSpeed; } }

    public float Turn(float deltaTime = 0)
    {
        Time += deltaTime;
        if (Time > Cycle) Time -= Cycle;

        return (float)(Amplitude * System.Math.Cos(AngularSpeed * Time + Phi));
    }
}
