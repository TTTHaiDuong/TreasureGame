using TreasureGame;
using Unity.Netcode;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private TimeCountDown TimeCountDown;
    [SerializeField] private WaitingRoom Room;
    [SerializeField] private Login Login;
    [SerializeField] private LogOut Out;
    [SerializeField] private RectTransform ScoreTable;
    [SerializeField] private EndOfTheGame EndGame;

    public static int PlayingTime;
    public static string RoomPassword;

    public static bool IsClient { get { return NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost; } }
    public static bool IsServer { get { return NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost; } }
    public static bool IsHost { get { return NetworkManager.Singleton.IsHost; } }

    private void Awake()
    {
        TimeCountDown.gameObject.SetActive(true);
        TimeCountDown.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Room.gameObject.activeSelf) Player.GetOwner().IsActive = false;
    }

    public void LoginToWaitingRoom()
    {
        Login.gameObject.SetActive(false);
        Room.SetActive(true);
        Out.gameObject.SetActive(true);
        TimeCountDown.gameObject.SetActive(false);
    }

    public void WaitingRoomToPlay()
    {
        if (Player.GetOwner() != null) Player.GetOwner().IsActive = true;
        Room.SetActive(false);
        Login.gameObject.SetActive(false);
        ScoreTable.gameObject.SetActive(true);
        TimeCountDown.gameObject.SetActive(true);
        Player.GetOwner().IsActive = true;
        ResetScoreAllPlayer();

        TimeCountDown.Timer.FinishListening(EndOfTheGame);
        TimeCountDown.Timer.Play(PlayingTime);
    }

    public void EndOfTheGame(object obj)
    {
        EndGame.SetActive(true);
        ScoreTable.gameObject.SetActive(false);
        Player.GetOwner().IsActive = false;

        SaveResult();
    }

    public void ReturnToWaitingRoom()
    {
        Room.SetActive(true);
        EndGame.SetActive(false);
    }

    public void LogOut()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            Application.Quit();
            return;
        }

        ScoreTable.gameObject.SetActive(false);
        Out.gameObject.SetActive(false);
        Room.SetActive(false);
        TimeCountDown.gameObject.SetActive(false);
        EndGame.SetActive(false);
        Login.gameObject.SetActive(true);
    }

    public void ResetScoreAllPlayer()
    {
        Player[] players = FindObjectsOfType<Player>();
        foreach (Player p in players)
            p.Score = 0;
    }

    public void SaveResult()
    {
        Player[] players = FindObjectsOfType<Player>();

        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            foreach (Player p in players)
                if (p.StudentId != "")
                {
                    Player.ExecuteQuery($"UPDATE {DatabaseManager.StudentTableName} " +
                        $"SET {DatabaseManager.StudentScoreColumn} = {DatabaseManager.StudentScoreColumn} + @score " +
                        $"WHERE {DatabaseManager.StudentIdColumn} = @studentId",
                        new string[] { "@score", "@studentId" }, new string[] { p.Score.ToString(), p.StudentId });

                    Player.ExecuteQuery($"UPDATE {DatabaseManager.StudentTableName} " +
                        $"SET {DatabaseManager.StudentAttendanceColumn} = {DatabaseManager.StudentAttendanceColumn} + @at " +
                        $"WHERE {DatabaseManager.StudentIdColumn} = @studentId",
                        new string[] { "@at", "@studentId" }, new string[] { 1.ToString(), p.StudentId });
                }
    }
}
