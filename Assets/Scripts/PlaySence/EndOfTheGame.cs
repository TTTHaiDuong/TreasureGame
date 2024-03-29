using TMPro;
using UnityEngine;

public class EndOfTheGame : MonoBehaviour, IUISetActive
{
    [SerializeField] private SceneManager Scenes; 
    [SerializeField] private TextMeshProUGUI Rating;
    [SerializeField] private ScoreTable ScoreTable;

    public void RankingPlayers()
    {
        Player[] players = ScoreTable.SortToScore();
        
        string rating = string.Empty;
        for (int i = 0; i < 3; i++)
            if (i < players.Length)
                rating += $"<color=#25FFFF><b>Top {i + 1}. {players[i].Name}          {players[i].Score}</b></color>\n";

        if (Player.GetOwner() != null && Player.GetOwner().Name != "")
        {
            int yourRating = ScoreTable.YourRaking(Player.GetOwner());
            rating += $"\n<b>You: {yourRating + 1}. {Player.GetOwner().Name}          {Player.GetOwner().Score}</b>";
        }

        Rating.text = rating;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        if (active) RankingPlayers();
    }
}
