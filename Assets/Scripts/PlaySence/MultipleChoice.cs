using GameUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private string[] Choices;
    private Player Player;
    private Map Map;

    private void Update()
    {
        ExitKey(Input.GetKeyDown(KeyCode.Escape));
    }

    public void Display(Player player, Map map)
    {
        Player = player;
        Map = map;

        Question.text = Map.GetQuestion(out Choices);
        Choice1.text = Choices[0];
        Choice2.text = Choices[1];
        Choice3.text = Choices[2];
        Choice4.text = Choices[3];

        if (Map.Penalty) Warning.gameObject.SetActive(true);
        Map.Timer.TickListening(CountDown);

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
        Player.Bag.AddRange(Map.Reward);
        Exit();
    }

    private void WrongAnswer()
    {
        if (Map.Penalty)
        {
            Player.LivingTimer.TickObj = "Trả lời câu hỏi không chính xác!";
            Player.Exploded(5);
        }
        Map.Pass = true;

        Exit();
    }

    private void CountDown(object obj)
    {
        LimitedTime.text = $"Còn: {Map.Timer.Time}";
        if (Map.Timer.Time == 0)
        {
            WrongAnswer();
            Map.Timer.Recall();
        }
    }

    public void Exit()
    {
        Clear();
        gameObject.SetActive(false);
        ReadMap.Exit();
    }

    private void ExitKey(bool keyInput)
    {
        if (keyInput) Exit();
    }
}
