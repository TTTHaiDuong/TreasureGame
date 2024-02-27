using System.Collections;
using System.Collections.Generic;
using TMPro;
using GameItems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System;
using TreasureGame;
using System.Data;
using System.Linq;
using UnityEngine.Networking;

namespace GameUI
{
    public class ReadMap : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI QuestionText;
        [SerializeField] private TextMeshProUGUI LimitTimeText;
        [SerializeField] private TextMeshProUGUI AnswerText;
        [SerializeField] private MultipleChoice MultipleChoices;
        [SerializeField] private Warning Warning;

        private Map IsReading;
        private Player Player;

        private void Update()
        {
            ExitInvoke(Input.GetKeyDown(KeyCode.Escape));
            CheckAnswer(Input.GetKeyDown(KeyCode.Return));
        }

        private void CheckAnswer(bool active)
        {
            if (active)
            {
                if (IsReading.Answer(AnswerText.text)) CorrectAnswer();
                else InCorrectAnswer(new object());
            }
        }

        public void ClearAll()
        {
            QuestionText.text = string.Empty;
            LimitTimeText.text = string.Empty;
            AnswerText.text = string.Empty;

            IsReading?.Timer.Recall();
            Player.SetActive(true);

            Warning.OnPointedExit();
            Warning.gameObject.SetActive(false);
        }

        public void Read(Player player, Map map)
        {
            gameObject.SetActive(true);
            IsReading = map;
            IsReading.Timer.TickListening(Counting);
            IsReading.Timer.FinishListening(InCorrectAnswer);

            Player = player;
            Player.SetActive(false);

            Warning.gameObject.SetActive(IsReading.Penalty);
            QuestionText.text = map.GetQuestion();
        }

        private void CorrectAnswer()
        {
            //Player.Bag.AddRange(QuestionFactory.GetReward(IsReading.Level));
            Player.Bag.AddRange(new Reward().Get("Items"));
            Exit();
        }

        private void InCorrectAnswer(object obj)
        {
            if (IsReading.Penalty) Penalty();
            Exit();
        }

        private void Penalty()
        {
            Player.Exploded(20);
        }

        private void Counting(object obj)
        {
            LimitTimeText.text = $"Còn: {IsReading.Timer.Time}";
        }

        private void ExitInvoke(bool active)
        {
            if (active) Exit();
        }

        public void Exit()
        {
            if (IsReading.IsLimitTime) InCorrectAnswer(true);
            ClearAll();
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
    //        Adapter = new SqlDataAdapter(query, Connection);
    //        DataSet = new DataSet();
    //        Adapter.Fill(DataSet);

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
        /// <param name="choices">Mặc định phần tử đầu tiên là đáp án.</param>
        public Map(string question, string answer, params string[] choices)
        {
            Question = question;
            Question = "Xin chao!";
            Result = answer;
            Choices = choices;
            Timer = new GameObject().AddComponent<Timer>();
        }

        public event TreasureHunt CorrectAnswer;
        public event TreasureHunt InCorrectAnswer;

        private readonly string Question;
        public readonly string Result;
        private readonly string[] Choices;
        public string[] OutChoices { private set; get; }

        public bool Pass;
        public int Level;
        public Timer Timer;
        public float LimitTime;
        public bool IsLimitTime;
        public bool IsOnce;
        public bool Penalty;
        public GameItem[] Reward;

        public string GetQuestion()
        {
            if (IsLimitTime) Timer.Play(LimitTime);
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
                WrongAnswer();
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
            CorrectAnswer?.Invoke();
            Pass = true;
        }

        private void WrongAnswer()
        {
            InCorrectAnswer?.Invoke();
            if (IsOnce) Pass = true;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override bool Compare(GameItem item)
        {
            throw new NotImplementedException();
        }
    }
}