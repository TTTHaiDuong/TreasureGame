using GameItems;
using UnityEngine;
using UnityEngine.UI;

public class UseItem : MonoBehaviour, IInitOwnerComponent
{
    public Player Player;
    public ItemImage ItemImage;
    public Image Image;
    public Island Island;

    void Update()
    {
        transform.forward = Player.PlayerCamera.transform.forward;
        PositionMatching();
        UseOnGetKey(Input.GetMouseButtonDown(0));
        CancelUseItem(Input.GetKeyDown(KeyCode.Escape));
    }

    private void UseOnGetKey(bool inputKey)
    {
        if (inputKey) UseOn();
    }

    private void CancelUseItem(bool inputKey)
    {
        if (inputKey) Cancel();
    }

    private void PositionMatching()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("TopBlock"))
        {
            Vector3 position = DefineBlock(hit.collider.bounds.center).transform.position;
            position.y += 4.5f;
            transform.position = position;
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
        }
    }

    public void UseOn()
    {
        ItemHandle(ItemImage.Item);
        Cancel();
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
        Cursor.visible = true;
    }

    public void ItemHandle(GameItem item)
    {
        Block block = DefineBlock(transform.position);
        switch (item.GetType().Name)
        {
            case "ThrownBomb":
                Bomb bomb = item as Bomb;
                bomb.IsReady = true;
                block.Decoration.Decorate(DecoratedBlock.New);
                block.RecoverTimer.Break();
                block.InitTimer.Break();
                block.Secret = new() { bomb };
                break;

            case "Glasses":
                break;

            case "SuperShovel":
                break;
        }
    }

    public void SetOwner(Player player)
    {
        Player = player;
    }

    public void RemoveOwner(Player player)
    {
        Player = null;
    }
}
