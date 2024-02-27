using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayUI : MonoBehaviour
{
    [SerializeField] private Player Player;

    public Sprite[] Badges;
    public Image Badge;
    public TextMeshProUGUI ScoreTable;
    public TextMeshProUGUI GoldTable;
    public TextMeshProUGUI RevivalCounting;
    public RectTransform OnBombPanel;

    private void Awake()
    {
        //Player = Player.Main;
    }

    private void Start()
    {
        //DisplayPlayer(Player);
    }

    public void DisplayPlayer(Player player)
    {
        Player = player;
        Player.LivingTimer.WhenStart -= OnBomb;
        Player.LivingTimer.Tick -= Counting;
        Player.LivingTimer.WhenFinish -= Revival;
        //Player.RevivalTimer.WhenBreak -= Revival;

        //if (Player.Main.Equals(player))
        {
            ScoreTable.text = player.Score.ToString();
            GoldTable.text = player.Gold.ToString();

            Player = player;
            Player.LivingTimer.WhenStart += OnBomb;
            Player.LivingTimer.Tick += Counting;
            Player.LivingTimer.WhenFinish += Revival;
            //Player.RevivalTimer.WhenBreak += Revival;

            SetBadge();
        }
    }

    private void OnBomb(object obj) => OnBombPanel.gameObject.SetActive(true);
    private void Revival(object obj) => OnBombPanel.gameObject.SetActive(false);

    public void SetBadge()
    {
        if (Player.Score < 10) Badge.sprite = Badges[0];
        else if (Player.Score < 20) Badge.sprite = Badges[1];
        else if (Player.Score < 30) Badge.sprite = Badges[2];
    }

    public void Counting(object obj)
    {
        RevivalCounting.text = $"{(string)obj}\n<size=120%><b>{Player.LivingTimer.Time:F0}</b></size>";
    }
}
