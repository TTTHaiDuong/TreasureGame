using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using TreasureGame;
using Unity.Netcode;
using UnityEngine;

public class Login : MonoBehaviour, IUISetActive
{
    [SerializeField] private SceneManager Scenes;
    [SerializeField] private TMP_InputField EmailField;
    [SerializeField] private TMP_InputField PasswordField;
    [SerializeField] private TextMeshProUGUI MessageText;

    public Timer MessageTimer;
    private static int ConnectionTime = 0;

    private void Awake()
    {
        MessageTimer = gameObject.AddComponent<Timer>();
        MessageTimer.StartListening((obj) => { MessageText.text = obj.ToString(); });
        MessageTimer.FinishListening((obj) => { MessageText.text = string.Empty; });
    }

    private void Update()
    {
        if (NetworkManager.Singleton.IsServer) Scenes.LoginToWaitingRoom();
    }

    public void EventSubmitClick()
    {
        string email = StringHandler.RemoveNonPrintChars(EmailField.text);
        string password = StringHandler.RemoveNonPrintChars(PasswordField.text);

        Player.ReceiceData -= CheckEmailPassword;
        Player.ReceiceData += CheckEmailPassword;
        Player.ExecuteQuery($"SELECT * FROM {DatabaseManager.AccountTableName} " +
            $"WHERE {DatabaseManager.AccountEmailColumnName} = @email " +
            $"AND {DatabaseManager.AccountPasswordColumnName} = @password",
            new string[] { "@email", "@password" }, new string[] { email, password });

        ConnectionTime++;
        if (ConnectionTime == DatabaseManager.ConnectionTime) CannotConnectToSQLServer();
    }

    private void CheckEmailPassword()
    {
        Player.ReceiceData -= CheckEmailPassword;
        DataTable tb = Player.Result;
        if (tb != null)
            foreach (DataColumn col in tb.Columns) Debug.Log(col.ColumnName);

        if (tb == null || !tb.Columns.Contains(DatabaseManager.AccountEmailColumnName))
        {
            CannotConnectToSQLServer();
            return;
        }

        if (tb.Rows.Count == 0)
        {
            MessageTimer.StartObj = "Đăng nhập không thành công!";
            MessageTimer.Play(3);
            return;
        }

        Debug.Log(tb.Rows[0][0].ToString());

        if (CheckLoggedIn(tb.Rows[0][0].ToString()))
        {
            MessageTimer.StartObj = "Tài khoản đã được đăng nhập!";
            MessageTimer.Play(3);
        }
        else
        {
            Player.GetOwner().IsLoggedIn = true;
            Player.GetOwner().StudentId = tb.Rows[0][0].ToString();
            Scenes.LoginToWaitingRoom();
        }
        ConnectionTime = 0;
    }

    private bool CheckLoggedIn(string studentId)
    {
        Player[] players = Player.FindPlayersWithCondition(p => p.IsLoggedIn);
        foreach (Player p in players)
            if (p != Player.GetOwner() && Player.GetOwner().StudentId == studentId) return true;

        return false;
    }

    private void CannotConnectToSQLServer()
    {
        Player.ReceiceData -= CheckEmailPassword;
        MessageTimer.StartObj = "Không thể kết nối đến máy chủ!";
        MessageTimer.Play(3);
        ConnectionTime = 0;
    }

    public void ReadAdminConfigFile(string path)
    {
        string[] lines = File.ReadAllLines(path);
        SceneManager.RoomPassword = lines[0];
        int.TryParse(lines[1], out SceneManager.PlayingTime);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        if (MessageTimer != null) MessageTimer.Promote();
    }
}

public class StringHandler
{
    public static string Simplify(string complexString)
    {
        return Regex.Replace(complexString, @"[^\u0020-\u007E]", string.Empty);
    }

    public static string RemoveNonPrintChars(string text)
    {
        return Regex.Replace(text, @"[^\p{IsBasicLatin}\p{IsLatin-1Supplement}\p{IsLatinExtended-A}\p{IsLatinExtended-B}\p{IsCombiningDiacriticalMarks}]+", "");
    }
}

public interface IUISetActive
{
    void SetActive(bool active);

    public static IUISetActive[] GetISetActiveMembers()
    {
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
        List<IUISetActive> list = new();

        foreach (GameObject obj in objects)
            if (obj.GetComponent<IUISetActive>() != null)
                list.Add(obj.GetComponent<IUISetActive>());

        return list.ToArray();
    }
}

public class ServerHostScaning
{
    private static Player[] ConnectedPlayers;
    private static readonly Timer ScanningTimer;
    private static readonly Timer ConnectingOrSetUpTimer;

    public static event Action AfterScanning;

    static ServerHostScaning()
    {
        ScanningTimer = new GameObject().AddComponent<Timer>();
        ScanningTimer.TickListening(IfAnyOtherConnectingsAreFound);
        ScanningTimer.FinishListening(ClientOrServer);

        ConnectingOrSetUpTimer = new GameObject().AddComponent<Timer>();
        ConnectingOrSetUpTimer.FinishListening(Start);
        ConnectingOrSetUpTimer.FinishListening((obj) => { GameObject.Destroy(ScanningTimer.gameObject); GameObject.Destroy(ConnectingOrSetUpTimer.gameObject); });

        ConnectedPlayers = new Player[0];
    }

    public static void Scan(float scanningTime)
    {
        NetworkManager.Singleton.Shutdown();
        if (!ScanningTimer.IsRunning)
        {
            NetworkManager.Singleton.StartClient();
            ScanningTimer.Play(scanningTime);
        }
    }

    private static void ClientOrServer(object obj)
    {
        Player[] remotes = GameObject.FindObjectsOfType<Player>();
        ConnectedPlayers = remotes;
        NetworkManager.Singleton.Shutdown();

        ConnectingOrSetUpTimer.Play(1);
    }

    private static void Start(object obj)
    {
        if (ConnectedPlayers.Length == 0 && File.Exists(@"C:\Users\tranh\OneDrive\Tài liệu\Desktop Application Development\TreasureAdmin.txt"))
        {
            NetworkManager.Singleton.StartHost();
            Resources.FindObjectsOfTypeAll<Login>()[0].ReadAdminConfigFile(@"C:\Users\tranh\OneDrive\Tài liệu\Desktop Application Development\TreasureAdmin.txt");
        }
        else NetworkManager.Singleton.StartClient();

        AfterScanning?.Invoke();
    }

    private static void IfAnyOtherConnectingsAreFound(object obj)
    {
        if (ConnectedPlayers.Length > 1)
            ScanningTimer.Promote();
    }
}
