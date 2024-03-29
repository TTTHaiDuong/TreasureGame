using GameUI;
using System.Data;
using System.IO;
using TreasureGame;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Quản lý thế giới trong game
/// </summary>
public class SceneManager : MonoBehaviour
{
    [SerializeField] private TimeCountDown TimeCountDown;
    [SerializeField] private WaitingRoom WaitingRoom;
    [SerializeField] private Login LoginObject;
    [SerializeField] private LogOut LogOutObject;
    [SerializeField] private ScoreTable ScoreTable;
    [SerializeField] private EndOfTheGame EndGame;
    [SerializeField] private Inventory Inventory;

    public const string SettingGameFile = @"C:\Users\tranh\OneDrive\Tài liệu\Desktop Application Development\TreasureAdmin.txt";

    public static DataTable Account;
    public static DataTable Question;

    public static bool EnterGame; // Chủ phòng cho phép vào game bằng cách gửi giá trị biến này đi khắp các máy khách
    public static int TimeToPlay; // Quy định thời gian chơi
    public static string RoomPassword; // Mật khẩu phòng

    public static bool IsClient { get { return NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer; } }

    private void Start()
    {
        ServerHostScaning.AfterScanning += EntryPoint;
        ServerHostScaning.Scan(2);
    }

    private void Update()
    {
        // Nếu như nhân vật tồn tại và được vào game IsEnterGame = true,
        // máy chủ đã bắt đầu game, lời yêu cầu của máy khách đến máy chủ được chấp nhận thì vào được game
        if (Player.GetOwner() != null && Player.GetOwner().Name != ""
            && EnterGame && WaitingRoom.RequestFlag
            && CheckPlayerNameRules(StringHandler.RemoveNonPrintChars(WaitingRoom.InputPlayerName.text)))
        {
            WaitingRoomToPlay();
            WaitingRoom.RequestFlag = false;
        }

        Account ??= DBProvider.ExecuteQuery($"SELECT * FROM {DBProvider.AccountTableName}");
        Question ??= DBProvider.ExecuteQuery($"SELECT * FROM {DBProvider.QuestionsTableName}");
    }

    /// <summary>
    /// Đọc file cài đặt của game (đối với máy chủ)
    /// </summary>
    public void ReadAdminConfigFile(string filePath)
    {
        const int len = 20;
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            if (line.Contains("Room Password_____: ")) RoomPassword = line[len..];
            else if (line.Contains("Connection String_: ")) DBProvider.ConnectionString = line[len..];
            else if (line.Contains("Time To Play______: ")) int.TryParse(line[len..], out TimeToPlay);
        }
    }

    /// <summary>
    /// Ẩn tất cả nhân vật bao gồm: tên nhân trống, chưa được vào phòng, chưa được đăng nhập, mã số sinh viên trống
    /// </summary>
    public void VisiblePlayers()
    {
        Player[] players = FindObjectsOfType<Player>();
        foreach (Player p in players)
            if (p.Name == "") p.SetActive(false);
            else if (p.Name != "" && !p.LivingTimer.IsRunning) p.SetActive(true);
    }

    /// <summary>
    /// Kiểm tra sự hợp lệ của tên nhân vật
    /// </summary>
    public bool CheckPlayerNameRules(string name)
    {
        if (name == "") return false;
        else return true;
    }

    /// <summary>
    /// Vô hiệu hoá tất cả các trang giao diện
    /// </summary>
    public void DisableUIs()
    {
        foreach (IUISetActive ui in IUISetActive.GetISetActiveMembers())
            ui.SetActive(false);
    }

    /// <summary>
    /// Điểm bắt đầu của các trang giao diện
    /// </summary>
    public void EntryPoint()
    {
        DisableUIs();
        LoginObject.SetActive(true);
        TimeCountDown.Init();
    }

    /// <summary>
    /// Đăng nhập vào phòng chờ
    /// </summary>
    public void LoginToWaitingRoom()
    {
        DisableUIs();
        WaitingRoom.SetActive(true);
        LogOutObject.SetActive(true);
    }

    /// <summary>
    /// Từ phòng chờ vào game
    /// </summary>
    public void WaitingRoomToPlay()
    {
        DisableUIs();
        ResetAllPlayer();

        ScoreTable.SetActive(true);
        TimeCountDown.SetActive(true);
        Inventory.SetActive(true);
        LogOutObject.SetActive(true);

        if (Player.GetOwner() != null)
        {
            Player.GetOwner().SetActive(true);
            Player.GetOwner().Island.InitIsland();
        }

        TimeCountDown.Timer.FinishListening(EndOfTheGame);
        TimeCountDown.Timer.Play(TimeToPlay);
    }

    /// <summary>
    /// Tổng kết, bảng điểm kết quả chơi
    /// </summary>
    public void EndOfTheGame(object obj)
    {
        DisableUIs();
        EndGame.SetActive(true);
        LogOutObject.SetActive(true);

        if (Player.GetOwner() != null)
        {
            Player.GetOwner().IsActive = false;
        }
        EnterGame = false;
        WaitingRoom.RequestFlag = false;
        SaveResult();
    }

    /// <summary>
    /// Đăng xuất
    /// </summary>
    public void LogOut()
    {
        Login.Student = null;
        if (NetworkManager.Singleton.IsServer) { Application.Quit(); } // Nếu như là máy server đăng xuất thì sẽ thoát ứng dụng
        EntryPoint();
    }

    /// <summary>
    /// Thiết đặt lại các thông số, kết quả của các người chơi
    /// </summary>
    public void ResetAllPlayer()
    {
        Player[] players = FindObjectsOfType<Player>();
        foreach (Player p in players)
        {
            p.Score = 0;
        }
    }

    /// <summary>
    /// Lưu kết quả của người chơi
    /// </summary>
    public void SaveResult()
    {
        Player[] players = Player.FindPlayersWithCondition(p => p.Score > 0 && p.StudentId != "");

        if (NetworkManager.Singleton.IsServer)
            foreach (Player p in players)
            {
                DBProvider.ExecuteQuery($"UPDATE {DBProvider.StudentTableName} " +
                    $"SET {DBProvider.StudentScoreColumn} = {DBProvider.StudentScoreColumn} + @score " +
                    $"WHERE {DBProvider.StudentIdColumn} = @studentId",
                    new string[] { "@score", "@studentId" }, new string[] { p.Score.ToString(), p.StudentId });

                DBProvider.ExecuteQuery($"UPDATE {DBProvider.StudentTableName} " +
                    $"SET {DBProvider.StudentAttendanceColumn} = {DBProvider.StudentAttendanceColumn} + @at " +
                    $"WHERE {DBProvider.StudentIdColumn} = @studentId",
                    new string[] { "@at", "@studentId" }, new string[] { 1.ToString(), p.StudentId });
            }
    }
}