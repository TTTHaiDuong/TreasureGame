using GameItems;
using UnityEngine;

namespace GameUI
{
    public class Inventory : MonoBehaviour, IInitOwnerComponent, IUISetActive
    {
        [SerializeField] private MultipleChoice MultipleChoice;
        public ItemInventory Bomb;
        public ItemInventory Glasses;
        public ItemInventory Shovel;

        public Player Player;

        private void Awake()
        {
            Bomb.TimeRecover = 10;
            Bomb.Do += UseBomb;

            Glasses.TimeRecover = 10;
            Glasses.Do += UseGlasses;

            Shovel.TimeRecover = 6;
            Shovel.Do += UseShovel;
        }

        public void UseBomb()
        {
            Player.ThrowBomb(true);
        }

        public void UseGlasses()
        {
            GameItems.Glasses.Detect(Player.BlockUnderFoot(), Player.Island.GetAllBlocks(), 9);
        }

        public void UseShovel()
        {
            Block[] blocks = GameItems.Glasses.DefineAround(Player.transform.position, Player.Island.GetAllBlocks(), 4);

            foreach (Block block in blocks)
            {
                block.Init();
                block.Secret = new()
                {
                      QuestionFactory.GetQuestion()
                };
            }
        }

        private void WaitToUse()
        {
            Bomb.Timer.Play(10);
            Glasses.Timer.Play(10);
            Shovel.Timer.Play(10);
        }

        public void SetOwner(Player player)
        {
            Player = player;
            Player.Inventory = this;
        }

        public void RemoveOwner(Player player)
        {
            Player = null;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            if (active) WaitToUse();
        }
    }
}
