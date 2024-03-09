using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameItems;
using TMPro;
using GameUI;

public class SelectedItem : MonoBehaviour
{
    public Inventory Inventory;
    public UseItem UseItem;
    public Image Image;
    public TextMeshProUGUI Properties;
    public TextMeshProUGUI Guide;
    public ItemImage ItemImage;
    public GameItem Item;

    public void Selected(ItemImage image)
    {
        ItemImage = image;
        Item = image.Item;
        Image.sprite = image.Image.sprite;
        Guide.text = Describe(image.Item);
        Properties.text = GetProperties(image.Item);
    }

    public void OnUseItem()
    {
        UseItem.StartDrag(ItemImage);
        Inventory.Open();
    }

    public string Describe(GameItem item)
    {
        return item.GetType().Name switch
        {
            "Bomb" => "Được sử dụng để đặt bẫy cho một block. Hữu ích cho chế độ multi-player, dùng để bẫy người chơi khác.",
            "Glasses" => "Được sử dụng để thăm dò đúng 9 block gần nhất, xem có mối nguy hiểm không.",
            "SuperShovel" => "Được sử dụng để đào một block chứa bẫy mà không phải chịu ảnh hưởng nào, hoặc bỏ qua thời gian hồi phục của block.",
            _ => ""
        };
    }

    public string GetProperties(GameItem item)
    {
        return item.GetType().Name switch
        {
            "Bomb" => $"Số lượng: {item.Count}\nVụ nổ: {(item as Bomb).Explode}",
            "Glasses" => $"Số lượng: {item.Count}",
            "SuperShovel" => $"Số lượng: {item.Count}",
            _ => ""
        };
    }
}
