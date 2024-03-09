using GameItems;
using GameUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayUI : MonoBehaviour
{
    [SerializeField] private Sprite[] Badges;
    [SerializeField] private Image Badge;
    [SerializeField] private TextMeshProUGUI ScoreTable;
    [SerializeField] private TextMeshProUGUI GoldTable;
    [SerializeField] private TextMeshProUGUI FoundMap;
    [SerializeField] private TextMeshProUGUI RevivalCountdownText;
    [SerializeField] private RectTransform OnBombPanel;

    public int TargetTreasure = 10;
    public Player Player;
    public Timer LimitedTime;

    private void Awake()
    {
        LimitedTime = new GameObject().AddComponent<Timer>();
        LimitedTime.TickListening(LimitedFindTreasure);
        LimitedTime.Play(600);
    }

    private void Update()
    {
        FoundMap.text = $"{Map.PassedQuestions}/{TargetTreasure}";
    }

    public void DisplayPlayer(Player player)
    {
        if (Player != null)
        {
            Player.LivingTimer.Recall();
            Player.Bag.ListChangedRecall(DisplayAchievements);
        }

        Player = player;
        Player.LivingTimer.StartListening(OnBomb);
        Player.LivingTimer.TickListening(RevivalCountdown);
        Player.LivingTimer.FinishListening(Revival);
        Player.Bag.ListChangedListening(DisplayAchievements);

        DisplayAchievements();
    }

    public void DisplayAchievements()
    {
        ScoreTable.text = GetPlayersScore().ToString();
        //GoldTable.text = GetPlayersGold().ToString();

        SetBadge();
    }

    public int GetPlayersGold()
    {
        int gold = 0;
        foreach (Gold item in Player.Bag.GetItems<Gold>())
            gold += item.Count;
        return gold;
    }

    public int GetPlayersScore()
    {
        int score = 0;
        foreach (Score item in Player.Bag.GetItems<Score>())
            score += item.Count;
        return score;
    }

    private void OnBomb(object obj) => OnBombPanel.gameObject.SetActive(true);
    private void Revival(object obj) => OnBombPanel.gameObject.SetActive(false);

    public void SetBadge()
    {
        if (GetPlayersScore() < 10) Badge.sprite = Badges[0];
        else if (GetPlayersScore() < 20) Badge.sprite = Badges[1];
        else if (GetPlayersScore() < 30) Badge.sprite = Badges[2];
    }

    public void RevivalCountdown(object obj)
    {
        RevivalCountdownText.text = $"{(string)obj}\n<size=120%><b>{Player.LivingTimer.Time:F0}</b></size>";
    }

    public void LimitedFindTreasure(object obj)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(LimitedTime.Time);
        if (LimitedTime.Time < 3600)
            GoldTable.text = $"{string.Format($"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}")}";
        else
            GoldTable.text = $"{string.Format($"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}")}";
    }
}
