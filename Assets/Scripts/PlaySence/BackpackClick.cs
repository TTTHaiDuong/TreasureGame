using GameUI;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackpackClick : MonoBehaviour
{
    public Inventory Inventory;

    public void OnPointerClick()
    {
        Inventory.Open();
    }
}
