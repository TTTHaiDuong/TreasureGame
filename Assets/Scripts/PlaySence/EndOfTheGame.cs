using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndOfTheGame : MonoBehaviour
{
    [SerializeField] private SceneManager Scenes; 
    [SerializeField] private TextMeshProUGUI Rating;
    [SerializeField] private ScoreTable ScoreTable;

    public void RatingPlayers()
    {
        Player[] players = ScoreTable.SortToScore();
        
        string rating = string.Empty;
        for (int i = 0; i < 3; i++)
            if (i < players.Length)
                rating += $"<color=#25FFFF><b>Top {i + 1}. {players[i].Name}     {players[i].Score}</b></color>\n";

        if (Player.GetOwner() != null)
        {
            int yourRating = ScoreTable.YourRating(Player.GetOwner());
            rating += $"\n<b>You: {yourRating + 1}. {Player.GetOwner().Name}     {players[yourRating].Score}</b>";
        }

        Rating.text = rating;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        RatingPlayers();
    }
}
