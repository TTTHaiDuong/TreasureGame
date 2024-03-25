using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreTable : MonoBehaviour, IInitOwnerComponent
{
    [SerializeField] private TextMeshProUGUI PlayerName;
    [SerializeField] private TextMeshProUGUI Score;
    [SerializeField] private TextMeshProUGUI OwnerName;
    [SerializeField] private TextMeshProUGUI OwnerScore;

    private Player Owner;
    
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

        string[] playersName = SortPlayes(format.ToArray(), "@text", out string[] scores);
        string ratingNames = string.Empty;
        string ratingScores = string.Empty;

        for (int i = 0; i < 3; i++)
            if (i < playersName.Length)
            {
                ratingNames += $"{playersName[i]}\n";
                ratingScores += $"{scores[i]}\n";
            }

        PlayerName.text = ratingNames;
        Score.text = ratingScores;

        SetOwnerRating();
    }

    public void SetOwnerRating()
    {
        if (Owner != null)
        {
            OwnerName.text = $"{YourRating(Player.GetOwner()) + 1}. {Owner.Name}\n";
            OwnerScore.text = $"{Owner.Score}\n";
        }
    }

    public int YourRating(Player player)
    {
        if (Owner != null)
        {
            Player[] players = FindObjectsOfType<Player>();
            for (int i = 0; i < players.Length; i++)
                if (players[i].Equals(player)) return i;
        }
        return 0;
    }

    public Player[] SortToScore()
    {
        Player[] players = FindObjectsOfType<Player>();

        List<Player> sortPlayers = players.ToList();
        sortPlayers.Sort((a, b) => a.Score.CompareTo(b.Score));

        return sortPlayers.ToArray();
    }

    public string[] SortPlayes(string[] formats, string replace, out string[] scores)
    {
        Player[] players = SortToScore();

        string[] outPutName = new string[players.Length];
        string[] outPutScore = new string[players.Length];

        for (int i = 0; i < outPutName.Length; i++)
        {
            if (i < formats.Length && formats[i] != null)
            {
                outPutName[i] = formats[i].Replace(replace, $"{i + 1}. {players[i].Name}");
                outPutScore[i] = formats[i].Replace(replace, $"{players[i].Score}");
            }
            else
            {
                outPutName[i] = $"{i + 1}. {players[i].Name}";
                outPutScore[i] = $"{players[i].Score}";
            }
        }

        scores = outPutScore;
        return outPutName;
    }

    public void SetOwner(Player player)
    {
        Owner = player;
    }

    public void RemoveOwner(Player player)
    {
        Owner = null;
    }
}
