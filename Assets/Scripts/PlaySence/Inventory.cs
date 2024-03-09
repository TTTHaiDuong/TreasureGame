using System.Collections;
using System.Collections.Generic;
using TreasureGame;
using Unity.VisualScripting;
using UnityEngine;
using GameItems;
using UnityEngine.UI;
using System;
using System.Drawing;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.EventSystems;

namespace GameUI
{
    public class Inventory : MonoBehaviour
    {
        public SelectedItem SelectedItem;
        public ItemImage ItemImage;
        public RectTransform BagPanel;

        public Vector2 InitialPosition;
        public Vector2 ContainerSize;
        public Vector2 ItemsSize;
        public float DistanceBetweenItems;

        public void Open()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().IsActive = !gameObject.activeSelf;

            PopUp();
        }

        public void PopUp()
        {
            //ItemList itemsList = Player.Main.Bag;
            ItemList itemList = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Bag;

            foreach (Transform clear in BagPanel)
                if (clear.CompareTag("ImageItem")) Destroy(clear.gameObject);

            int k = 0;
            for (int i = 0; i < ContainerSize.y; i++)
                for (int j = 0; j < ContainerSize.x; j++)
                {
                    if (k >= itemList.Count) return;
                    Vector3 position = InitialPosition;
                    position += new Vector3(DistanceBetweenItems + (DistanceBetweenItems + ItemsSize.x) * j,
                        -DistanceBetweenItems - (DistanceBetweenItems + ItemsSize.y) * i);

                    ItemImage addItem = Instantiate(ItemImage);
                    addItem.Init(itemList[k], ItemsSize);
                    AssignEvent(addItem);

                    addItem.RectImage.anchorMax = new Vector2(0, 1);
                    addItem.RectImage.anchorMin = new Vector2(0, 1);
                    addItem.RectImage.pivot = new Vector2(0, 1);

                    addItem.RectImage.anchoredPosition = position;
                    addItem.transform.SetParent(BagPanel.transform, false);

                    k++;
                }
        }

        private void AssignEvent(ItemImage image)
        {
            EventTrigger eventTrigger = image.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => Selected((PointerEventData)data));
            eventTrigger.triggers.Add(entry);
        }

        private void Selected(PointerEventData e)
        {
            ItemImage image = e.pointerPress.GetComponent<ItemImage>();
            SelectedItem.Selected(image);
        }
    }
}
