using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Trang đăng nhập game
/// </summary>
public class Login : MonoBehaviour, IUISetActive
{
    [SerializeField] private SceneManager Scenes; // Quản lý trang trò chơi
    [SerializeField] private TMP_InputField EmailField; // Trường nhập email
    [SerializeField] private TMP_InputField PasswordField; // Trường nhập mật khẩu
    [SerializeField] private TextMeshProUGUI MessageText; // Thông báo

    public Timer MessageTimer; // Timer bật tắt thông báo trong thời gian nhất định
    public static DataRow Student;

    private void Awake()
    {
        MessageTimer = gameObject.AddComponent<Timer>();
        MessageTimer.StartListening((obj) => { MessageText.text = obj.ToString(); });
        MessageTimer.FinishListening((obj) => { MessageText.text = string.Empty; });
    }

    private void Update()
    {
        // Nếu như là máy chủ thì bỏ qua bước đăng nhập
        if (NetworkManager.Singleton.IsServer) Scenes.LoginToWaitingRoom();
    }

    /// <summary>
    /// Kiểm tra email và password, liên kết với sự kiện click nút Đăng nhập
    /// </summary>
    public void CheckEmailPassword()
    {
        string email = StringHandler.RemoveNonPrintChars(EmailField.text);
        string password = StringHandler.RemoveNonPrintChars(PasswordField.text);

        // Nếu như bảng Account bị trống thì thông báo lỗi
        if (SceneManager.Account == null)
        {
            MessageTimer.StartObj = "Không thể kết nối đến máy chủ!";
            MessageTimer.Play(3);
        }
        // Nếu đúng email và mật khẩu
        else if (IscorrectEmailPassword(email, password, out DataRow found))
        {
            // Nếu tài khoản đã được đăng nhập
            if (CheckLoggedIn(found[0].ToString()))
            {
                MessageTimer.StartObj = "Tài khoản đã được đăng nhập!";
                MessageTimer.Play(3);
            }
            // Nếu như tài khoản chưa được đăng nhập
            else
            {
                Student = found;
                Scenes.LoginToWaitingRoom();
            }
        }
        // Nếu như sai tài khoản hay mật khẩu
        else
        {
            MessageTimer.StartObj = "Thông tin đăng nhập không chính xác!";
            MessageTimer.Play(3);
        }
    }

    /// <summary>
    /// Kiểm tra tính đúng đắn của thông tin đăng nhập
    /// </summary>
    /// <param name="email">Email</param>
    /// <param name="password">Mật khẩu</param>
    /// <param name="foundRow">Dòng chứa email và mật khẩu đó trong CSDL</param>
    /// <returns>Nếu thông tin chính xác là true, ngược lại false</returns>
    private bool IscorrectEmailPassword(string email, string password, out DataRow foundRow)
    {
        foreach (DataRow row in SceneManager.Account.Rows)
            if (row[2].ToString() == email && row[3].ToString() == password)
            {
                foundRow = row;
                return true;
            }
        foundRow = null;
        return false;
    }

    /// <summary>
    /// Kiểm tra tài khoản có được đăng nhập chưa
    /// </summary>
    /// <param name="studentId">Mã số sinh viên</param>
    /// <returns>True: tài khoản đã đăng nhập, ngược lại false</returns>
    private bool CheckLoggedIn(string studentId)
    {
        if (Student == null) return false;
        else if (Student[0].ToString() != studentId) return false;
        else return true;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        MessageTimer.Promote();
    }
}

/// <summary>
/// Xử lý chuỗi
/// </summary>
public class StringHandler
{
    /// <summary>
    /// Đơn giản hoá các kí tự unicode, xoá bỏ dấu của chữ tiếng Việt
    /// </summary>
    public static string Simplify(string complexString)
    {
        return Regex.Replace(complexString, @"[^\u0020-\u007E]", string.Empty);
    }

    /// <summary>
    /// Xoá bỏ các kí tự không in ra được trong unicode
    /// </summary>
    public static string RemoveNonPrintChars(string text)
    {
        return Regex.Replace(text, @"[^\p{IsBasicLatin}\p{IsLatin-1Supplement}\p{IsLatinExtended-A}\p{IsLatinExtended-B}\p{IsCombiningDiacriticalMarks}]+", "");
    }
}

/// <summary>
/// Active của các trang ứng dụng
/// </summary>
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

/// <summary>
/// Thệ thống quét các thiết bị mạng
/// </summary>
public class ServerHostScaning
{
    /* Tóm tắt:
     * Sử dụng phương thức Scan để bắt đầu một máy chủ hay máy khách
     * Máy này thành lập một máy khách giả để tìm kiếm máy chủ
     * Nếu tìm thấy máy chủ thì giữ nguyên trạng thái là máy khách
     * Nếu không tìm thấy bất kì máy nào khác và trong máy tính có file cài đặt game thì thành lập máy chủ
     * Kết thúc
     */

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

    /// <summary>
    /// Bắt đầu quét
    /// </summary>
    public static void Scan(float scanningTime)
    {
        NetworkManager.Singleton.Shutdown();
        if (!ScanningTimer.IsRunning)
        {
            NetworkManager.Singleton.StartClient();
            ScanningTimer.Play(scanningTime);
        }
    }

    // Đếm số lượng các máy kết nối được
    private static void ClientOrServer(object obj)
    {
        Player[] remotes = GameObject.FindObjectsOfType<Player>();
        ConnectedPlayers = remotes;
        if (ConnectedPlayers.Length > 1) return;
        NetworkManager.Singleton.Shutdown();

        ConnectingOrSetUpTimer.Play(1);
    }

    // Bắt đầu một máy server hay client
    private static void Start(object obj)
    {
        if (ConnectedPlayers.Length == 0 && File.Exists(SceneManager.SettingGameFile))
        {
            NetworkManager.Singleton.StartHost();
            Resources.FindObjectsOfTypeAll<SceneManager>()[0].ReadAdminConfigFile(SceneManager.SettingGameFile);
        }
        else NetworkManager.Singleton.StartClient();

        AfterScanning?.Invoke();
    }

    // Vượt qua thời gian quét nếu tìm thấy bất kì máy nào
    private static void IfAnyOtherConnectingsAreFound(object obj)
    {
        if (ConnectedPlayers.Length > 1)
            ScanningTimer.Promote();
    }
}
