using System.Collections.Generic;
using TMPro;
using GameItems;
using UnityEngine;
using TreasureGame;
using System.Linq;

namespace GameUI
{
    public class ReadMap : MonoBehaviour
    {
        [SerializeField] private MultipleChoice MultipleChoice;
        [SerializeField] private ShortAnswer ShortAnswer;

        private bool IsShortAnswer;
        private Player Player;
        private Map Map;

        public void Display(Player player, Map map)
        {
            Player = player;
            Player.IsActive = false;

            if (Map != map) IsShortAnswer = new GameRandom().Probability(0);
            Map = map;
            if (IsShortAnswer || Map.Choices.Length != 3) ShortAnswer.Display(Player, Map);
            else MultipleChoice.Display(Player, Map);

            gameObject.SetActive(true);
        }

        private void CompleteChallenge()
        {
            Block block = Player.BlockUnderFoot();
            block.FloatingItem.Picked();
        }

        public void Exit()
        {
            Player.IsActive = true;
            if (Map.Pass) CompleteChallenge();
            gameObject.SetActive(false);
        }
    }

    //public class DataBase
    //{
    //    private readonly string ConnectionString;
    //    private readonly SqlConnection Connection;
    //    private SqlDataAdapter Adapter;
    //    private DataSet DataSet;

    //    public DataBase()
    //    {
    //        ConnectionString = "";
    //        Connection = new SqlConnection(ConnectionString);
    //    }

    //    public DataTable Execute(string query)
    //    {
    //        Adapter = new SqlDataAdapter(query, selectConnection: Connection);
    //        DataSet = new DataSet();

    //        return DataSet.Tables[0];
    //    }

    //    public void ExecuteNonQuery(string query)
    //    {
    //        SqlCommand sqlcmd = new(query, Connection);
    //        Connection.Open();
    //        sqlcmd.ExecuteNonQuery();
    //        Connection.Close();
    //    }
    //}

    //public class QuestionFactory
    //{
    //    static QuestionFactory()
    //    {
    //        List<LevelField> levelFields = new()
    //        {
    //            new(), // Nhận biết
    //            new(), // Thông hiểu
    //            new(), // Vận dụng thấp
    //            new(), // Vận dụng cao
    //            new(), // Phân tích
    //            new(), // Tổng hợp
    //            new()  // Đánh giá
    //        };

    //        LevelFields = levelFields.ToArray();
    //    }

    //    private readonly string QuestionsTableName = "";
    //    private readonly string LevelColumnName = "";
    //    private readonly string QuestionColumnName = "";
    //    private readonly string AnswerColumnName = "";
    //    private readonly string Choice1ColumnName = "";
    //    private readonly string Choice2ColumnName = "";
    //    private readonly string Choice3ColumnName = "";

    //    public static LevelField[] LevelFields;
    //    public DataBase DataBase = new();

    //    public Map InitMap(int level)
    //    {
    //        foreach (LevelField field in LevelFields)
    //            if (field.Level == level)
    //            {
    //                GameRandom rd = new();
    //                Map map = new("", "")
    //                {
    //                    Level = level,
    //                    IsLimitTime = rd.Probability(field.IsLimitTimeRate),
    //                    LimitTime = rd.Next(field.LimitTimeInterval),
    //                    IsOnce = rd.Probability(field.IsOnceRate),
    //                    Penalty = rd.Probability(field.PenaltyRate),
    //                    Reward = field.Reward
    //                };
    //                return map;
    //            }
    //        return null;
    //    }

    //    public DataTable GetQuestions(int level)
    //    {
    //        string query = $"SELECT * FROM {QuestionsTableName} WHERE {LevelColumnName} = {level}";
    //        return DataBase.Execute(query);
    //    }

    //    public static GameItem[] GetReward(int level)
    //    {
    //        foreach (LevelField field in LevelFields)
    //            if (field.Level == level) return field.Reward;
    //        return null;
    //    }
    //}

    public struct LevelField
    {
        public LevelField(int level, float isLimitTimeRate, float isOnceRate, float penaltyRate, KeyValuePair<int, int> limitTimeInterval, GameItem[] reward)
        {
            Level = level;
            IsLimitTimeRate = isLimitTimeRate;
            IsOnceRate = isOnceRate;
            PenaltyRate = penaltyRate;
            LimitTimeInterval = limitTimeInterval;
            Reward = reward;
        }
        public int Level;
        public KeyValuePair<int, int> LimitTimeInterval;
        public float IsLimitTimeRate;
        public float IsOnceRate;
        public float PenaltyRate;
        public GameItem[] Reward;
    }

    public class Map : GameItem
    {
        public Map(string question, string answer, params string[] choices)
        {
            Question = question;
            Result = answer;
            Choices = choices;
            Timer = new GameObject().AddComponent<Timer>();
            Count = 1;
        }

        public static int PassedQuestions;

        private readonly string Question;
        public readonly string Result;
        public readonly string[] Choices;
        public string[] OutChoices { private set; get; }

        public Timer Timer;

        public bool Pass;
        public float LimitedTime;
        public bool IsOnce;
        public bool Penalty;
        public GameItem[] Reward;

        public void Setter(float limitedTime, bool isOnce, bool penalty, GameItem[] reward)
        {
            LimitedTime = limitedTime;
            IsOnce = isOnce;
            Penalty = penalty;
            Reward = reward;
        }

        public string GetQuestion()
        {
            if (LimitedTime > 0) Timer.Play(LimitedTime);
            return Question;
        }

        public string GetQuestion(out string[] choices)
        {
            OutChoices = Choices.Concat(new string[] { Result }).ToArray();
            new GameRandom().Shuffle(OutChoices);
            choices = OutChoices;
            return GetQuestion();
        }

        public bool Answer(string answer)
        {
            if (!Pass && CheckAnswer(answer))
            {
                RightAnswer();
                return true;
            }
            else
            {
                if (!Pass) WrongAnswer();
                return false;
            }
        }

        public bool Answer(int choice)
        {
            return Answer(OutChoices[choice]);
        }

        private bool CheckAnswer(string answer)
        {
            return true;
        }

        private void RightAnswer()
        {
            PassedQuestions++;
            Pass = true;
        }

        private void WrongAnswer()
        {
            if (IsOnce) Pass = true;
        }

        public override string ToString()
        {
            return Question;
        }

        #region Phép so sánh và sao chép
        public static bool operator ==(Map a, Map b)
        {
            return (a is null && b is null) || (a is not null && b is not null && a.Question == b.Question && a.Result == b.Result
                && a.Choices == b.Choices && a.Pass == b.Pass && a.LimitedTime == b.LimitedTime
                && a.IsOnce == b.IsOnce && a.Penalty == b.Penalty && a.Reward == b.Reward);
        }

        public static bool operator !=(Map a, Map b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == obj as Map;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override object Clone()
        {
            return new Map(Question, Result, Choices);
        }
        #endregion
    }
}