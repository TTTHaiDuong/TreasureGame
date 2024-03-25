using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using TreasureGame;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class Login : MonoBehaviour
{
    [SerializeField] private SceneManager Scenes;
    [SerializeField] private TMP_InputField EmailField;
    [SerializeField] private TMP_InputField PasswordField;
    [SerializeField] private TextMeshProUGUI MessageText;

    public static int ConnectionTime = 0;

    private void Start()
    {
        ServerHostScaning.Scan();
    }

    private void Update()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            NextScene();
        }
    }

    public void SubmitClick()
    {
        string email = StringHandler.RemoveNonPrintChars(EmailField.text);
        string password = StringHandler.RemoveNonPrintChars(PasswordField.text);

        Player.ReceiceData += CheckEmailPassword;
        Player.ExecuteQuery($"SELECT * FROM {DatabaseManager.AccountTableName} WHERE {DatabaseManager.AccountEmailColumnName} = @email AND {DatabaseManager.AccountPasswordColumnName} = @password", new string[] { "@email", "@password" }, new string[] { email, password });

        ConnectionTime++;
        if (ConnectionTime == DatabaseManager.ConnectionTime) CannotConnectToSQLServer();
    }

    private void CheckEmailPassword()
    {
        Player.ReceiceData -= CheckEmailPassword;
        DataTable tb = Player.Result;
        if (tb == null) SubmitClick();

        if (tb.Rows.Count == 0) MessageText.text = "Đăng nhập không thành công!";
        else
        {
            Player.GetOwner().StudentId = tb.Rows[0][0].ToString();
            MessageText.text = "Đăng nhập thành công!";
            NextScene();
        }

        ConnectionTime = 0;
    }

    private void CannotConnectToSQLServer()
    {

    }

    private void NextScene()
    {
        Scenes.LoginToWaitingRoom();
    }

    public void ReadAdminConfigFile(string path)
    {
        string[] lines = File.ReadAllLines(path);
        SceneManager.RoomPassword = lines[0];
        int.TryParse(lines[1], out SceneManager.PlayingTime);
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

public class ServerHostScaning
{
    private static Player[] Players;
    private static Timer Timer1;
    private static Timer Timer2;

    public static int CountClient;
    public static bool Flag;

    static ServerHostScaning()
    {
        Timer1 = new GameObject().AddComponent<Timer>();
        Timer1.FinishListening(ClientOrServer);
    }

    public static void Scan()
    {
        NetworkManager.Singleton.Shutdown();
        if (!Timer1.IsRunning)
        {
            NetworkManager.Singleton.StartClient();
            Timer1 = new GameObject().AddComponent<Timer>();
            Timer1.TickListening(Tick);
            Timer1.FinishListening(ClientOrServer);
            Timer1.Play(2);

            Timer2 = new GameObject().AddComponent<Timer>();
            Timer2.FinishListening(Start);
        }
    }

    private static void ClientOrServer(object obj)
    {
        Player[] remotes = GameObject.FindObjectsOfType<Player>();
        Players = remotes;
        NetworkManager.Singleton.Shutdown();

        CountClient = remotes.Length;
        Flag = true;

        Timer2.Play(1);
    }

    private static void Start(object obj)
    {
        if (CountClient == 0 && File.Exists(@"C:\Users\tranh\OneDrive\Tài liệu\Desktop Application Development\TreasureAdmin.txt"))
        {
            NetworkManager.Singleton.StartHost();
            GameObject.FindObjectOfType<Login>().ReadAdminConfigFile(@"C:\Users\tranh\OneDrive\Tài liệu\Desktop Application Development\TreasureAdmin.txt");
        }
        else NetworkManager.Singleton.StartClient();
    }

    private static void Tick(object obj)
    {
        if (Players != null && Players.Length > 1)
            Timer1.Time = 0;
    }
}
