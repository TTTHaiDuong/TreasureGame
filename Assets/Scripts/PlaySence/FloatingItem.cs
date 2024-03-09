using GameItems;
using GameUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FloatingItem : MonoBehaviour
{
    private HarmonicOscillation Osc;
    private Vector3 Root;

    [SerializeField] private ItemImage ItemImage;
    [SerializeField] private Image Image;
    [SerializeField] private RectTransform RectImage;

    public GameItem Item;

    private void Awake()
    {
        Osc = new(0.2f, 5);
        RectImage = Image.GetComponent<RectTransform>();
        Root = RectImage.anchoredPosition;
    }

    void Update()
    {
        Image.transform.forward = Camera.main.transform.forward;
        Oscillation();
    }

    private void Oscillation()
    {
        Vector3 pos = Root;
        pos.y = Osc.Turn(Time.deltaTime) + Root.y;
        RectImage.anchoredPosition = pos;
    }

    public void Floating(ItemImage item)
    {
        Image.sprite = item.Image.sprite;
        Item = item.Item.Clone() as GameItem;
        gameObject.SetActive(true);
    }

    public void Floating(GameItem item)
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
