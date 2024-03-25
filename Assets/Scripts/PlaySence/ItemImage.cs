using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace GameItems
{
    public class ItemImage : MonoBehaviour
    {
        private Vector2 RatioImageToCountTable;

        [SerializeField] private Sprite[] ItemSprites;

        public Image Image;
        public TextMeshProUGUI CountText;

        public RectTransform RectImage;
        public RectTransform RectCount;

        public GameItem Item;

        private void Awake()
        {
            RectImage = Image.GetComponent<RectTransform>();
            RectCount = CountText.GetComponent<RectTransform>();
            RatioImageToCountTable = new Vector2(RectImage.sizeDelta.x / RectCount.sizeDelta.x, RectImage.sizeDelta.y / RectCount.sizeDelta.y);
        }

        public void Init(GameItem item, Vector2 size)
        {
            Item = item.Clone() as GameItem;
            Image.sprite = GetSprite(item);
            SetSize(size);
            SetCount(item.Count);
        }

        public void SetSize(Vector2 size)
        {
            RectImage.sizeDelta = size;
            RectCount.sizeDelta = new Vector2(size.x / RatioImageToCountTable.x, size.y / RatioImageToCountTable.y);
        }

        public void SetCount(int count) => CountText.text = count.ToString();

        public Sprite GetSprite(GameItem item)
        {
            return item.GetType().Name switch
            {
                "Gold" => ItemSprites[0],
                "ThrownBomb" => ItemSprites[1],
                "SuperShovel" => ItemSprites[2],
                "Glasses" => ItemSprites[3],
                "Map" => ItemSprites[4],
                _ => null
            };
        }
    }

    public abstract class GameItem : ICloneable
    {
        public GameItem(int count = 0)
        {
            Count = count;
        }

        public int Count;
        public abstract object Clone();
    }

    public class Bomb : GameItem
    {
        public Bomb(int count = 0, uint explode = 0) : base(count)
        {
            Explode = explode;
        }

        public uint Explode;
        public bool IsReady;

        public void Active(int count, params Player[] players)
        {
            if (count > Count) return;
            Count -= count;
            foreach (Player player in players)
                player.Exploded(Explode * count);
        }

        public static bool operator ==(Bomb a, Bomb b)
        {
            return (a is null && b is null) || (a is not null && b is not null && a.Count == b.Count && a.Explode == b.Explode && a.IsReady == b.IsReady);
        }

        public static bool operator !=(Bomb a, Bomb b)
        {
            return !(a == b);
        }

        public override bool Equals(object item)
        {
            return this == item as Bomb;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override object Clone() => new Bomb(Count, Explode);
    }

    public class Glasses : GameItem
    {
        public Glasses(int count = 0) : base(count) { }

        public static void Detect(Block block, Block[] setOfBlocks, int area)
        {
            Block[] areaDetected = DefineAround(block.transform.position, setOfBlocks, area);
            foreach (Block detectBlock in areaDetected)
                if (BombDetection(detectBlock)) detectBlock.Text.SetText("!", 10);
        }

        public static int CountOfBombsAround(Block block, Block[] setOfBlocks, int area)
        {
            int count = 0;
            Block[] areaDetected = DefineAround(block.transform.position, setOfBlocks, area);
            foreach (Block detectBlock in areaDetected)
                if (BombDetection(detectBlock)) count++;
            return count;
        }

        public static Block[] DefineAround(Vector3 center, Block[] setOfBlocks, int count)
        {
            List<Block> result = setOfBlocks.ToList();
            result.Sort((a, b) => Vector3.Distance(center, a.transform.position).CompareTo(Vector3.Distance(center, b.transform.position)));
            return result.GetRange(0, count).ToArray();
        }

        public static bool BombDetection(Block block)
        {
            foreach (GameItem item in block.Secret)
                if (item is Bomb bomb && bomb.IsReady) return true;
            return false;
        }

        public static bool operator ==(Glasses a, Glasses b)
        {
            return (a is null && b is null) || (a is not null && b is not null && a.Count == b.Count);
        }

        public static bool operator !=(Glasses a, Glasses b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == obj as Glasses;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override object Clone() => new Glasses(Count);
    }

    public class SuperShovel : GameItem
    {
        public SuperShovel(int count = 0) : base(count) { }
        public GameItem[] Use(Block block)
        {
            if (Count > 0)
            {
                Count--;
                GameItem[] get = (GameItem[])block.Secret.ToArray().Clone();
                block.Init();

                Island island = block.transform.parent.GetComponent<Island>();
                Block[] blocks = island.GetAllBlocks();
                //block.SetBlockText(Glasses.CountOfBombsAround(block, blocks, 9).ToString(), 10);

                return get;
            }
            else return null;
        }

        public static bool operator ==(SuperShovel a, SuperShovel b)
        {
            return (a is null && b is null) || (a is not null && b is not null && a.Count == b.Count);
        }

        public static bool operator !=(SuperShovel a, SuperShovel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == obj as SuperShovel;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override object Clone() => new SuperShovel(Count);
    }

    public class Gold : GameItem
    {
        public Gold(int count = 0) : base(count) { }

        public bool SendTo(Gold other, int count)
        {
            if (count > Count) return false;
            {
                Count -= count;
                other.Count += count;
                return true;
            }
        }

        public static bool operator ==(Gold a, Gold b)
        {
            return (a is null && b is null) || (a is not null && b is not null && a.Count == b.Count);
        }

        public static bool operator !=(Gold a, Gold b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == obj as Gold;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override object Clone() => new Gold(Count);
    }

    public class Score : GameItem
    {
        public Score(int count = 0) : base(count) { }

        public override object Clone() => new Score(Count);

        public static bool operator ==(Score a, Score b)
        {
            return (a is null && b is null) || (a is not null && b is not null && a.Count == b.Count);
        }

        public static bool operator !=(Score a, Score b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == obj as Score;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    //public class TreasureMap : GameItem
    //{
    //    public TreasureMap(string question, string answer, int level)
    //    {
    //        Question = "Bạn tên là gì?";
    //        Answer = "Tran Hai Duong";
    //        Level = level;

    //        GameObject obj = new();
    //        LimitTimer = obj.AddComponent<Timer>();
    //        LimitTimer.WhenFinish += (a) =>
    //        {
    //            Pass = true;
    //            WrongAnswer?.Invoke();
    //        };
    //        Init();
    //    }

    //    public event TreasureHunt RightAnswer;
    //    public event TreasureHunt WrongAnswer;

    //    private readonly string Question;

    //    public static int Target;

    //    public Timer LimitTimer;
    //    public ItemsList Rewards;
    //    public bool IsOnce;
    //    public int Level;
    //    public int Time;
    //    public bool Penalty;

    //    public bool Pass { get; private set; }
    //    public string Answer { get; private set; }

    //    public string GetQuestion()
    //    {
    //        if (Time != 0 && !LimitTimer.IsRunning) LimitTimer.Counting(Time);
    //        return Question;
    //    }

    //    /// <summary>
    //    /// Thử nghiệm.
    //    /// </summary>
    //    public void Solve(string answer)
    //    {
    //        if (Pass) return;
    //        if (Check(answer))
    //        {
    //            Pass = true;
    //            RightAnswer?.Invoke();
    //        }
    //        else
    //        {
    //            if (IsOnce) Pass = true;
    //            WrongAnswer?.Invoke();
    //        }
    //    }

    //    public bool Check(string answer)
    //    {
    //        string original = Regex.Replace(Answer, @"[^\u0020-\u007E]", string.Empty).ToLower();
    //        answer = Regex.Replace(answer, @"[^\u0020-\u007E]", string.Empty).ToLower();
    //        if (original == answer) return true;
    //        else return false;
    //    }

    //    //
    //    // Level:
    //    // 0: Nhận biết
    //    // 1: Hiểu
    //    // 2: Vận dụng thấp
    //    // 3: Vận dụng cao
    //    // 4: Phân tích
    //    // 5: Tổng hợp
    //    // 6: Đánh giá
    //    //
    //    public void Init()
    //    {
    //        IsOnce = false;
    //        Rewards = new();
    //        GameRandom rd = new();
    //        Penalty = false;

    //        if (rd.Probability(GameConst.Penalty)) Penalty = true;

    //        if (Level >= 0 && Level <= 2)
    //        {
    //            ItemsList items = new()
    //            {
    //                new Bomb(1, (uint)rd.ChooseFromList(GameConst.Explosion)),
    //                new Glasses(1),
    //                new SuperShovel(1),
    //                new Gold(rd.Next(GameConst.Funds))
    //            };

    //            Rewards.Add(new Score(1));
    //            Rewards.Add(rd.ChooseFromList(items.ToArray()));

    //            if (rd.Probability(GameConst.HaveLimitTimeEasy))
    //            {
    //                Time = rd.ChooseFromList(GameConst.LimitTimeEasy);
    //                IsOnce = true;
    //            }
    //            IsOnce = new GameRandom().Probability(GameConst.OnceAnswer);
    //        }
    //        else
    //        {
    //            ItemsList items = new()
    //            {
    //                new Bomb(rd.Next(2, 5), (uint)rd.ChooseFromList(GameConst.Explosion)),
    //                new Glasses(rd.Next(2, 5)),
    //                new SuperShovel(rd.Next(2, 5)),
    //                new Gold(rd.Next(GameConst.Funds))
    //            };

    //            Rewards.Add(new Score(2));
    //            Rewards.Add(rd.ChooseFromList(items.ToArray()));

    //            if (rd.Probability(GameConst.HaveLimitTimeDifficult))
    //            {
    //                Time = rd.ChooseFromList(GameConst.LimitTimeDifficult);
    //                IsOnce = true;
    //            }
    //            IsOnce = new GameRandom().Probability(GameConst.OnceAnswer);
    //        }
    //    }

    //    public override object Clone() => new TreasureMap(Question, Answer, Level);
    //    public override bool Compare(GameItem item) => Equals(item as TreasureMap);
    //}
}