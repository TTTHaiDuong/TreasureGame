using System.Collections.Generic;
using UnityEngine;
using TreasureGame;
using GameItems;
using GameUI;
using System.Linq;
using Unity.Netcode;

public class Block : MonoBehaviour
{
    [SerializeField] private List<TopBlock> List;

    public DecoratedTopBlocks Decoration;
    public TopBlock TopBlock;
    public TextOfBlock Text;
    public ItemList Secret;
    public Explosion Explosion;
    public ReadMap ReadMap;
    public FloatingItem FloatingItem;

    public Timer InitTimer;
    public Timer RecoverTimer;

    private void Awake()
    {
        QuestionFactory init = new();

        RecoverTimer = gameObject.AddComponent<Timer>();
        InitTimer = gameObject.AddComponent<Timer>();

        RecoverTimer.FinishListening((obj) => Init());
        InitTimer.FinishListening((obj) => Init());
        Decoration = new DecoratedTopBlocks(this, List);
    }

    private void Start()
    {
        Init();
        RecoverTimer.Break();
    }

    public void DugUp(Player player)
    {
        if (!RecoverTimer.IsRunning)
        {
            Decoration.Decorate(DecoratedBlock.Grass);
            if (TakeItems(player)) return;
            DetectBombAround(9);

            RecoverTimer.Play(new GameRandom().Next(GameConst.RecoverInterval));
            InitTimer.Break();
        }
    }

    public bool TakeItems(Player player)
    {
        foreach (GameItem item in Secret)
        {
            if (item is Bomb bomb && bomb.IsReady)
            {
                ActiveExplosion();
                player.LivingTimer.TickObj = "Bạn đã đào phải mìn!";
                bomb.Active(bomb.Count, player);
            }
            else if (item is Map map && !map.Pass)
            {
                FloatingItem.SetUp(item);
                ReadMap.Display(player, map);

                return true;
            }
        }
        Secret.Clear();
        return false;
    }

    public void ActiveExplosion()
    {
        Vector3 pos = transform.position;
        pos.y += 4.5f;
        Explosion.Play(Explosion.Duration / 2, pos);
    }

    public void DetectBombAround(int area)
    {
        Island island = transform.parent.GetComponent<Island>();
        int count = Glasses.CountOfBombsAround(this, island.GetAllBlocks(), area);
        Text.SetText(count.ToString(), 10);
    }

    public void Init()
    {
        InitSecret();
        Text.Clear();
        Decoration.Decorate(DecoratedBlock.New);
        RecoverTimer.Break();
        InitTimer.Play(new GameRandom().Next(GameConst.InitInterval));
    }

    public void InitSecret()
    {
        Reward reward = new();

        Dictionary<object, double> suprisingThings = new()
        {
            { reward.Get("Questions"), 0.5 },
            { reward.Get("IsTrap"), 0.15 },
            { reward.Get("Golds"), 0.2 },
            { reward.Get("Items"), 0.1 }
        };

        Secret = new();
        Secret.AddRange((GameItem[])new GameRandom().Probability(suprisingThings.ToArray()));
    }
}

public enum DecoratedBlock
{
    New,
    Grass
}

public class Reward
{
    public GameItem[] Get(string ticket)
    {
        GameRandom rd = new();
        switch (ticket)
        {
            case "IsTrap":
                Bomb bomb = new(1, (uint)rd.ChooseFromList(new int[] { 3, 4, 5 }))
                { IsReady = true };
                return new GameItem[] { bomb };

            case "Golds":
                Gold gold = new(rd.Next(GameConst.Funds));
                return new GameItem[] { gold };

            case "Items":
                List<GameItem> gameItems = new()
            {
                new Bomb(1, (uint)rd.ChooseFromList(GameConst.Explosion)),
                new Glasses(1),
                new SuperShovel(1)
            };
                return new GameItem[] { rd.ChooseFromList(gameItems.ToArray()) };

            case "Questions":
                Map map = QuestionFactory.GetQuestion();
                return new GameItem[] { map };

            default:
                return null;
        }
    }
}

public class DecoratedTopBlocks : List<TopBlock>
{
    public DecoratedTopBlocks(Block owner, List<TopBlock> list)
    {
        Origin = owner;
        Init(list);

        Decorate(DecoratedBlock.New);
    }

    public Vector3 Position { get { return Origin.TopBlock.transform.position; } }
    public Block Origin;

    public void Init(List<TopBlock> list)
    {
        Clear();
        foreach (TopBlock block in list)
        {
            TopBlock clone = Object.Instantiate(block);
            clone.transform.SetParent(Origin.transform);
            clone.transform.position = Position;
            clone.gameObject.SetActive(false);
            Add(clone);
        }
    }

    public void Decorate(DecoratedBlock type)
    {
        if (type < 0 || (int)type >= Count) return;

        Origin.TopBlock.gameObject.SetActive(false);
        Origin.TopBlock = this[(int)type];
        Origin.TopBlock.gameObject.SetActive(true);
        Origin.TopBlock.OnMouseExit();
    }
}