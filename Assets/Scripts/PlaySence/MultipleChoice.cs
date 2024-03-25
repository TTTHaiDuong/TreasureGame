using GameUI;
using TMPro;
using TreasureGame;
using UnityEngine;

public class MultipleChoice : MonoBehaviour
{
    [SerializeField] private ReadMap ReadMap;

    [SerializeField] private TextMeshProUGUI Question;
    [SerializeField] private TextMeshProUGUI LimitedTime;
    [SerializeField] private Warning Warning;

    [Header("Các text trắc nghiệm")]
    [SerializeField] private TextMeshProUGUI Choice1;
    [SerializeField] private TextMeshProUGUI Choice2;
    [SerializeField] private TextMeshProUGUI Choice3;
    [SerializeField] private TextMeshProUGUI Choice4;

    public Timer Timer;
    private string[] Choices;
    private Player Player;
    private Map Map;

    private void Awake()
    {
        Timer = new GameObject().AddComponent<Timer>();
        Timer.TickListening(CountDown);
        Timer.FinishListening(Penalty);
    }

    private void Update()
    {
        ExitKey(Input.GetKeyDown(KeyCode.Escape));
    }

    public void Display(Player player, Map map)
    {
        if (map == null) return;
        if (Map != map)
        {
            Clear();
            Player = player;
            Map = map;

            Question.text = Map.GetQuestion(out Choices);
            Choice1.text = Choices[0];
            Choice2.text = Choices[1];
            Choice3.text = Choices[2];
            Choice4.text = Choices[3];

            if (new GameRandom().Probability(0.3) && Timer != null && !Timer.IsRunning)
            {
                Warning.gameObject.SetActive(true);
                Timer.Play(new GameRandom().Next(10, 20));
            }
        }

        Player.IsActive = false;
        gameObject.SetActive(true);
    }

    private void Clear()
    {
        LimitedTime.text = string.Empty;
        Warning.OnPointedExit();
        Warning.gameObject.SetActive(false);
    }

    public void Button1()
    {
        if (Map.Answer(0)) RightAnswer();
        else WrongAnswer();
    }

    public void Button2()
    {
        if (Map.Answer(1)) RightAnswer();
        else WrongAnswer();
    }

    public void Button3()
    {
        if (Map.Answer(2)) RightAnswer();
        else WrongAnswer();
    }

    public void Button4()
    {
        if (Map.Answer(3)) RightAnswer();
        else WrongAnswer();
    }

    private void RightAnswer()
    {
        Timer.Break();
        Player.Score += new GameRandom().Next(5, 10);
        RemoveMap();
        Exit();
    }

    private void WrongAnswer()
    {
        RemoveMap();
        Exit();
    }

    private void Penalty(object obj)
    {
        Player.LivingTimer.TickObj = "Trả lời câu hỏi không chính xác!";
        Player.Exploded(5);
        Player.Score -= new GameRandom().Next(1, 4);
        Player.BlockUnderFoot().ActiveExplosion();
        Timer.Break();
        RemoveMap();
        Exit();
    }

    private void CountDown(object obj)
    {
        LimitedTime.text = $"Còn: {Timer.Time}";
    }

    public void Exit()
    {
        Clear();
        Player.IsActive = true;
        gameObject.SetActive(false);
        ReadMap.Exit();
    }

    private void ExitKey(bool keyInput)
    {
        if (keyInput) Exit();
    }

    private void RemoveMap()
    {
        if (Map != null)
        {
            Map.Pass = true;
            Map = null;
        }
    }
}
