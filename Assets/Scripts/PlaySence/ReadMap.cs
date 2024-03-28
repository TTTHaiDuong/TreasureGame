using GameItems;
using UnityEngine;
using TreasureGame;
using System.Linq;
using System.Data;

namespace GameUI
{
    public class ReadMap : MonoBehaviour
    {
        [SerializeField] private MultipleChoice MultipleChoice;
        private Player Player;
        private Map Map;

        public void Display(Player player, Map map)
        {
            Map = map;
            Player = player;
            gameObject.SetActive(true);
            MultipleChoice.Display(player, map);
        }

        private void CompleteChallenge()
        {
            Block block = Player.BlockUnderFoot();
            block.FloatingItem.Picked();
        }

        public void Exit()
        {
            if (Map.Pass) CompleteChallenge();
            gameObject.SetActive(false);
        }
    }

    public class QuestionFactory
    {
        static QuestionFactory()
        {
            GetData();
        }

        public static int ConnectionTime = 0;
        public static DataTable QuestionTable;

        public static void GetData() // Bắt đầu lấy dữ liệu
        {
            Player.ReceiceData += ReceiveData;
            Player.ExecuteQuery($"SELECT * FROM {DatabaseManager.QuestionsTableName}", new string[0], new string[0]);

            ConnectionTime++;
            if (ConnectionTime == DatabaseManager.ConnectionTime) CannotConnectToSQLServer();
        }

        public static void ReceiveData() // Nhận được dữ liệu
        {
            Player.ReceiceData -= ReceiveData;
            QuestionTable = Player.Result;
            if (QuestionTable == null || QuestionTable.Rows.Count == 0 || !QuestionTable.Columns.Contains(DatabaseManager.QuestionColumn)) CannotConnectToSQLServer();
            // Nếu như bảng dữ liệu không hợp lệ như là null, số dòng bằng 0, tên cột không chính xác
        }

        public static Map GetQuestion() // Các lớp khác truy xuất câu hỏi
        {
            if (QuestionTable == null || QuestionTable.Rows.Count == 0) return new("Hãy chọn đáp án!", "A", "B", "C", "D");
            int rd = new GameRandom().Next(QuestionTable.Rows.Count);
            DataRow row = QuestionTable.Rows[rd];
            Map map = new(row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString(), row[5].ToString());

            ConnectionTime = 0;
            return map;
        }

        public static void CannotConnectToSQLServer() // Không thể kết nối đến SQL server
        {
            Player.ReceiceData -= ReceiveData;
            ConnectionTime = 0;
        }
    }

    public class Map : GameItem
    {
        public Map(string question, string answer, params string[] choices)
        {
            Question = question;
            Result = answer;
            Choices = choices;
            Count = 1;
        }

        public static int PassedQuestions;

        public string Question;
        public string Result;
        public string[] Choices;
        public string[] OutChoices { private set; get; }

        public bool Pass;

        public string GetQuestion(out string[] choices)
        {
            OutChoices = Choices.Concat(new string[] { Result }).ToArray();
            new GameRandom().Shuffle(OutChoices);
            choices = OutChoices;
            return Question;
        }

        public bool Answer(string answer)
        {
            return !Pass && CheckAnswer(answer);
        }

        public bool Answer(int choice)
        {
            return Answer(OutChoices[choice]);
        }

        private bool CheckAnswer(string answer)
        {
            if (answer == Result) return true;
            else return false;
        }

        public override object Clone()
        {
            return new Map(Question, Result, Choices);
        }
    }
}