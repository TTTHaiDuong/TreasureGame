using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreTable : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PlayerName;
    [SerializeField] private TextMeshProUGUI Score;
    [SerializeField] private TextMeshProUGUI OwnerName;
    [SerializeField] private TextMeshProUGUI OwnerScore;

    public void Update()
    {
        Rating();
    }

    public void Rating()
    {
        List<string> format = new()
        {
            "<color=yellow><b>@text</b></color>",
            "<color=#25FFFF><b>@text</b></color>",
            "<color=#FF7500><b>@text</b></color>",
        };

        string[] playerNames = GetPlayerNamesRanking(out string[] scores);
        string rankingNameText = string.Empty;
        string rankingScoreText = string.Empty;

        for (int i = 0; i < format.Count; i++)
            if (i < playerNames.Length)
            {
                rankingNameText += $"{format[i].Replace("@text", playerNames[i])}\n";
                rankingScoreText += $"{format[i].Replace("@text", scores[i])}\n";
            }

        PlayerName.text = rankingNameText;
        Score.text = rankingScoreText;

        SetOwnerRaking();
    }

    public void SetOwnerRaking()
    {
        if (Player.GetOwner() != null && Player.GetOwner().Name != "")
        {
            OwnerName.text = $"{YourRaking(Player.GetOwner()) + 1}. {Player.GetOwner().Name}\n";
            OwnerScore.text = $"{Player.GetOwner().Score}\n";
        }
    }

    public int YourRaking(Player player)
    {
        if (Player.GetOwner() != null && Player.GetOwner().Name != "")
        {
            Player[] players = Player.FindPlayersWithCondition(p => p.Name != "");
            for (int i = 0; i < players.Length; i++)
                if (players[i].Equals(player)) return i;
        }
        return -1;
    }

    public Player[] SortToScore()
    {
        Player[] players = Player.FindPlayersWithCondition(p => p.Name != "");
        List<Player> sortPlayers = players.ToList();
        sortPlayers.Sort((a, b) => a.Score.CompareTo(b.Score));

        return sortPlayers.ToArray();
    }

    public string[] GetPlayerNamesRanking(out string[] scores)
    {
        Player[] players = SortToScore();

        string[] outPutName = new string[players.Length];
        string[] outPutScore = new string[players.Length];

        for (int i = 0; i < outPutName.Length; i++)
        {
            outPutName[i] = $"{i + 1}. {players[i].Name}";
            outPutScore[i] = $"{players[i].Score}";
        }
        scores = outPutScore;
        return outPutName;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
