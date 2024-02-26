using GameItems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseItem : MonoBehaviour
{
    public Player Player;
    public ItemImage ItemImage;
    public Image Image;
    public Island Island;
    public bool IsDragged;

    void Update()
    {
        transform.forward = Camera.main.transform.forward;
        if (IsDragged)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UseOn();
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("TopBlock"))
            {
                Vector3 position = DefineBlock(hit.collider.bounds.center).transform.position;
                position.y += 4.5f;
                transform.position = position;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
    }

    public Block DefineBlock(Vector3 point)
    {
        return Glasses.DefineAround(point, Island.GetAllBlocks(), 1)[0];
    }

    public void StartDrag(ItemImage image)
    {
        if (image != null)
        {
            ItemImage = image;
            Image.sprite = image.Image.sprite;

            gameObject.SetActive(true);
            Cursor.visible = false;
            IsDragged = true;
        }
    }

    public void UseOn()
    {
        Plan(ItemImage.Item);
        for (int i = 0; i < Player.Bag.Count; i++)
            if (Player.Bag[i].Count == 0) Player.Bag.Remove(Player.Bag[i]);
        Cancel();
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
        Cursor.visible = true;
        IsDragged = false;
    }

    public void Plan(GameItem item)
    {
        Block block = DefineBlock(transform.position);
        switch (item.GetType().Name)
        {
            case "Bomb":
                block.Decoration.Decorate(DecoratedBlock.New);
                block.RecoverTimer.Break();
                block.InitTimer.Break();
                block.Secret = new() { item };
                //block.IsDanger = true;
                break;
            case "Glasses":
                if (item is Glasses glasses) 
                    glasses.Detect(block, Island.GetAllBlocks(), 9);
                break;
            case "SuperShovel":
                break;

        }
    }
}
