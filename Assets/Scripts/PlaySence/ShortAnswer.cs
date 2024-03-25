using GameUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//public class ShortAnswer : MonoBehaviour
//{
//    [SerializeField] private ReadMap ReadMap;
//    [SerializeField] private TextMeshProUGUI Question;
//    [SerializeField] private TextMeshProUGUI LimitedTime;
//    [SerializeField] private TMP_InputField AnswerText;
//    [SerializeField] private Warning Warning;

//    private Player Player;
//    private Map Map;

//    private void Update()
//    {
//        Answer(Input.GetKeyDown(KeyCode.Return));
//        ExitKey(Input.GetKeyDown(KeyCode.Escape));
//    }

//    public void Display(Player player, Map map)
//    {
//        Player = player;
//        Map = map;

//        Question.text = map.GetQuestion();
//        AnswerText.text = string.Empty;
//        if (Map.Penalty) Warning.gameObject.SetActive(true);
//        Map.Timer.TickListening(CountDown);

//        gameObject.SetActive(true);
//    }

//    private void Clear()
//    {
//        LimitedTime.text = string.Empty;
//        Warning.OnPointedExit();
//        Warning.gameObject.SetActive(false);
//    }

//    public void Answer(bool keyInput)
//    {
//        if (keyInput)
//        {
//            if (Map.Answer(AnswerText.text)) RightAnswer();
//            else WrongAnswer();
//        }
//    }

//    private void RightAnswer()
//    {
//        //Player.Bag.AddRange(Map.Reward);
//        Exit();
//    }

//    private void WrongAnswer()
//    {
//        if (Map.Penalty)
//        {
//            Player.LivingTimer.TickObj = "Trả lời không chính xác!";
//            Player.Exploded(5);
//        }
//        Exit();
//    }

//    private void CountDown(object obj)
//    {
//        LimitedTime.text = $"Còn: {Map.Timer.Time}";
//        if (Map.Timer.Time == 0)
//        {
//            WrongAnswer();
//            Map.Timer.Recall();
//        }
//    }

//    public void Exit()
//    {
//        Clear();
//        gameObject.SetActive(false);
//        ReadMap.Exit();
//    }

//    private void ExitKey(bool keyInput)
//    {
//        if (keyInput) Exit();
//    }
//}
