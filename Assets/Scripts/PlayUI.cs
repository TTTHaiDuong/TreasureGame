using GameItems;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayUI : MonoBehaviour
{
    public Player Player;
    public Sprite[] Badges;
    public Image Badge;
    public TextMeshProUGUI ScoreTable;
    public TextMeshProUGUI GoldTable;
    public TextMeshProUGUI RevivalCountdownText;
    public RectTransform OnBombPanel;

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
        GoldTable.text = GetPlayersGold().ToString();

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
}
