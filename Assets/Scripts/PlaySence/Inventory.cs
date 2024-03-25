using GameItems;
using UnityEngine;

namespace GameUI
{
    public class Inventory : MonoBehaviour, IInitOwnerComponent
    {
        [SerializeField] private MultipleChoice MultipleChoice;
        [SerializeField] private ItemInventory Bomb;
        [SerializeField] private ItemInventory Glasses;
        [SerializeField] private ItemInventory Shovel;

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

        private void Update()
        {
            if (Player != null && Player.IsActive)
            {
                Glasses.Use(Input.GetKeyDown(KeyCode.Alpha1));
                Shovel.Use(Input.GetKeyDown(KeyCode.Alpha2));
                Bomb.Use(Input.GetKeyDown(KeyCode.Alpha3));
            }
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

        public void StartGame()
        {
            Bomb.Timer.Play(10);
            Glasses.Timer.Play(10);
            Shovel.Timer.Play(10);
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
}
