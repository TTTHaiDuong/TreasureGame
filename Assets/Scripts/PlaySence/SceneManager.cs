using GameUI;
using TreasureGame;
using Unity.Netcode;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private TimeCountDown TimeCountDown;
    [SerializeField] private WaitingRoom WaitingRoom;
    [SerializeField] private Login LoginObject;
    [SerializeField] private LogOut LogOutObject;
    [SerializeField] private ScoreTable ScoreTable;
    [SerializeField] private EndOfTheGame EndGame;
    [SerializeField] private Inventory Inventory;

    public static bool EnterGame;
    public static int PlayingTime;
    public static string RoomPassword;

    public static bool IsClient { get { return NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer; } }
    public static bool IsServer { get { return NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient; } }
    public static bool IsHost { get { return NetworkManager.Singleton.IsHost; } }

    private void Start()
    {
        ServerHostScaning.AfterScanning += EntryPoint;
        ServerHostScaning.Scan(2);
    }

    private void Update()
    {
        if (Player.GetOwner() != null && !Player.GetOwner().IsEnterGame
            && EnterGame && WaitingRoom.RequestFlag
            && CheckPlayerNameRules(StringHandler.RemoveNonPrintChars(WaitingRoom.InputPlayerName.text)))
        {
            WaitingRoomToPlay();
        }
        VisiblePlayers();
    }

    private void VisiblePlayers()
    {
        Player[] players = FindObjectsOfType<Player>();

        foreach (Player p in players)
            if (p.IsEnterGame && IsClient) p.Visible(true);
            else p.Visible(false);
    }

    public bool CheckPlayerNameRules(string name)
    {
        if (name == "") return false;
        else return true;
    }

    public void DisableUIs()
    {
        foreach (IUISetActive ui in IUISetActive.GetISetActiveMembers())
            ui.SetActive(false);
    }

    public void EntryPoint()
    {
        DisableUIs();
        LoginObject.SetActive(true);
        TimeCountDown.Init();
    }

    public void LoginToWaitingRoom()
    {
        Debug.Log("Wait");

        DisableUIs();
        WaitingRoom.SetActive(true);
        LogOutObject.SetActive(true);

        Player.GetOwner().Island.Flag = false;
    }

    public void WaitingRoomToPlay()
    {
        Debug.Log("Play");

        DisableUIs();
        ResetScoreAllPlayer();
        ScoreTable.SetActive(true);
        TimeCountDown.SetActive(true);
        Inventory.SetActive(true);
        LogOutObject.SetActive(true);

        TimeCountDown.Timer.FinishListening(EndOfTheGame);
        TimeCountDown.Timer.Play(PlayingTime);

        if (Player.GetOwner() != null)
        {
            Player.GetOwner().IsActive = true;
            Player.GetOwner().IsEnterGame = true;
        }
    }

    public void EndOfTheGame(object obj)
    {
        Debug.Log("End");

        DisableUIs();
        EndGame.SetActive(true);
        LogOutObject.SetActive(true);

        if (Player.GetOwner() != null)
        {
            Player.GetOwner().IsActive = false;
            Player.GetOwner().IsEnterGame = false;
        }
        EnterGame = false;
        WaitingRoom.RequestFlag = false;
        SaveResult();
    }

    public void LogOut()
    {
        if (NetworkManager.Singleton.IsServer) { Application.Quit(); return; }
        EntryPoint();
    }

    public void ResetScoreAllPlayer()
    {
        Player[] players = FindObjectsOfType<Player>();
        foreach (Player p in players) p.Score = 0;
    }

    public void SaveResult()
    {
        Player[] players = Player.FindPlayersWithCondition(p => p.IsEnterGame && p.IsClient && !p.IsHost && p.StudentId != "");

        if (NetworkManager.Singleton.IsServer)
            foreach (Player p in players)
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
