using GameItems;
using UnityEngine;
using TreasureGame;
using System.Linq;
using System.Data;

namespace GameUI
{
    /// <summary>
    /// Đọc bản đồ kho báu
    /// </summary>
    public class ReadMap : MonoBehaviour
    {
        [SerializeField] private MultipleChoice MultipleChoice;
        private Player Player;
        private Map Map;

        /// <summary>
        /// Hiển thị
        /// </summary>
        /// <param name="player">Người chơi trả lời</param>
        /// <param name="map">Bản đồ</param>
        public void Display(Player player, Map map)
        {
            Map = map;
            Player = player;
            gameObject.SetActive(true);
            MultipleChoice.Display(player, map);
        }

        // Hoàn thành "thử thách" trong bản đồ
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
        public static DataTable QuestionTable;

        /// <summary>
        /// Truy xuất câu hỏi
        /// </summary>
        /// <returns></returns>
        public static Map GetQuestion()
        {
            if (QuestionTable == null || QuestionTable.Rows.Count == 0) return new("Để lấy giá trị của các cột của hàng thứ i trong một đối tượng DataTable có tên là datatable.", "Object[] array = datatable.Rows[i].ItemArray;", "DataRow array = datatable. Rows[i].ItemArray;", "DataColumn array = datatable. Rows[i].ItemArray;", "String[] array = datatable. Rows[i].ItemArray;");
            int rd = new GameRandom().Next(QuestionTable.Rows.Count);
            DataRow row = QuestionTable.Rows[rd];
            Map map = new(row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString(), row[5].ToString());
            return map;
        }
    }

    /// <summary>
    /// Lớp bản đồ kho báu, dùng để chứa thông tin về câu hỏi để người chơi trả lời nhận điểm
    /// </summary>
    public class Map : GameItem
    {
        public Map(string question, string answer, params string[] choices)
        {
            Question = question;
            Result = answer;
            Choices = choices;
            Count = 1;
        }

        public string Question;
        public string Result;
        public string[] Choices;
        public string[] OutChoices { private set; get; }

        public bool Pass; // Vượt qua câu hỏi true: đã vượt qua, ngược lại false

        /// <summary>
        /// Nhận cầu hỏi
        /// </summary>
        public string GetQuestion(out string[] choices)
        {
            OutChoices = Choices.Concat(new string[] { Result }).ToArray();
            new GameRandom().Shuffle(OutChoices);
            choices = OutChoices;
            return Question;
        }

        /// <summary>
        /// Trả lời
        /// </summary>
        public bool Answer(string answer)
        {
            return !Pass && CheckAnswer(answer);
        }

        /// <summary>
        /// Trả lời theo dạng trắc nghiệm
        /// </summary>
        public bool Answer(int choice)
        {
            return Answer(OutChoices[choice]);
        }

        /// <summary>
        /// Kiểm tra kết quả
        /// </summary>
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