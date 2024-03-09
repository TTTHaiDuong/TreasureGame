﻿using System.Collections.Generic;
using UnityEngine;
using TreasureGame;
using TMPro;
using GameItems;
using GameUI;
using Unity.VisualScripting;
using System.Net.Sockets;
using System.Linq;
using UnityEditorInternal.Profiling.Memory.Experimental;

//public class Block : MonoBehaviour
//{
//    public GameObject OriginTopBlock;

//    public ReadMap ReadMap;
//    public Explosion ExplosionParticle;
//    public TextMeshProUGUI Text;
//    public GameObject[] TopBlocks;
//    public GameObject Display;

//    public Timer RecoverTimer;
//    public Timer InitTimer;
//    public Timer TextTimer;

//    public ItemList Secret;
//    public bool IsDanger;

//    private void Awake()
//    {
//        RecoverTimer = gameObject.AddComponent<Timer>();
//        RecoverTimer.WhenFinish += AutoInit;

//        InitTimer = gameObject.AddComponent<Timer>();
//        InitTimer.WhenFinish += AutoInit;

//        TextTimer = gameObject.AddComponent<Timer>();
//        TextTimer.WhenFinish += ClearText;

//        Secret = new();
//    }

//    private void Start()
//    {
//        RecoverTimer.TimeRemaining = 0;

//        Vector3 explosionPosition = transform.position;
//        explosionPosition.y += 4.5f;
//        ExplosionParticle.transform.position = explosionPosition;

//        Init();
//    }

//    public void Decorate(int type)
//    {
//        if (type == -1)
//        {
//            OriginTopBlock.SetActive(true);
//            OriginTopBlock.GetComponent<TopBlock>().OnMouseExit();

//            if (Display != null) Display.SetActive(false);
//        }
//        else
//        {
//            OriginTopBlock.SetActive(false);

//            if (Display != null) Destroy(Display);
//            Display = Instantiate(TopBlocks[type]);
//            Display.SetActive(true);
//            Display.transform.position = OriginTopBlock.transform.position;
//        }
//    }

//    private void AutoInit(object obj)
//    {
//        SetBlockText("");
//        Init();
//    }

//    private void ClearText(object obj)
//    {
//        Text.text = string.Empty;
//    }

//    public void DugUpBy(Player player)
//    {
//        if (!RecoverTimer.IsRunning)
//        {
//            Decorate(0);

//            foreach (TreasureMap map in Secret.GetItems<TreasureMap>())
//            {
//                if (map.Pass) continue;
//                else
//                {
//                    InitTimer.TimeRemaining += map.Time + 10;
//                    ReadMap.Read(player, map);
//                    return;
//                }
//            }

//            if (IsDanger)
//            {
//                player.RevivalTimer.TickParameter = "Bạn đã đào phải mìn!";
//                ExplosionParticle.Play(ExplosionParticle.Duration / 2);

//                Bomb[] bombs = Secret.GetItems<Bomb>();
//                foreach (Bomb bomb in bombs)
//                {
//                    bomb.Active(bomb.Count, player);
//                    Secret.Remove(bomb);
//                }
//            }

//            player.GetItems(Secret);

//            Island island = transform.parent.GetComponent<Island>();
//            Block[] blocks = island.GetAllBlocks();
//            SetBlockText(Glasses.CountOfBombsAround(this, blocks, 9).ToString(), 10);

//            if (new GameRandom().Probability(GameConst.HaveTimeToRecover))
//                RecoverTimer.Counting(new GameRandom().Next(GameConst.RecoverInterval));
//            else RecoverTimer.Counting(20);
//            InitTimer.Break();

//            IsDanger = false;
//        }
//    }

//    public void SetBlockText(string text, float second = 0)
//    {
//        Text.text = text;
//        if (second != 0) TextTimer.Counting(second);
//    }

//    public void Init()
//    {
//        Decorate(-1);

//        GameRandom rd = new();
//        Secret = new();
//        IsDanger = false;

//        if (rd.Probability(GameConst.IsTrap))
//        {
//            IsDanger = true;
//            Bomb bomb = new(1, (uint)rd.ChooseFromList<int>(GameConst.Explosion));
//            Secret.Add(bomb);
//        }
//        else if (rd.Probability(GameConst.Golds))
//        {
//            Gold gold = new(rd.Next(GameConst.Funds));
//            Secret.Add(gold);
//        }
//        else if (rd.Probability(GameConst.Items))
//        {
//            List<GameItem> gameItems = new()
//            {
//                new Bomb(1, (uint)rd.ChooseFromList<int>(GameConst.Explosion)),
//                new Glasses(1),
//                new SuperShovel(1)
//            };
//            Secret.Add(rd.ChooseFromList(gameItems.ToArray()));
//        }
//        else if (rd.Probability(GameConst.Questions))
//        {
//            TreasureMap map = new("", "", 2);
//            Secret.Add(map);
//        }

//        InitTimer.Counting(new GameRandom().Next(GameConst.InitInterval));
//        RecoverTimer.Break();
//    }
//}

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
                Vector3 pos = transform.position;
                pos.y += 4.5f;
                Explosion.Play(Explosion.Duration / 2, pos);

                player.LivingTimer.TickObj = "Bạn đã đào phải mìn!";
                bomb.Active(bomb.Count, player);
            }
            else if (item is Map map && !map.Pass)
            {
                FloatingItem.Floating(item);
                ReadMap.Display(player, map);

                return true;
            }
            else
            {
                player.Bag.Add(item);
            }
        }
        Secret.Clear();
        return false;
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
                Map map = new("Bạn tên là gì?", "A", "B", "C", "D");
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
            TopBlock clone = GameObject.Instantiate(block);
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