using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Phòng chờ
/// </summary>
public class WaitingRoom : MonoBehaviour, IUISetActive
{
    [SerializeField] private SceneManager Scene;
    [SerializeField] private TextMeshProUGUI CountPlayer;
    [SerializeField] private TextMeshProUGUI PlayerList;

    [SerializeField] private RectTransform StartButton;
    [SerializeField] private RectTransform ClientControls;
    [SerializeField] private RectTransform ServerControls;
    [SerializeField] private TMP_InputField Password;

    public TMP_InputField InputPlayerName;
    public bool RequestFlag; // Người chơi gửi lời yêu cầu vào phòng game nếu đã đúng các thông tin cần có trong game

    private void Update()
    {
        Listed();
    }

    /// <summary>
    /// Liệt kê danh sách người chơi
    /// </summary>
    private void Listed()
    {
        Player[] players = Player.FindPlayersWithCondition(p => p.Name != "");

        CountPlayer.text = $"Hiện tại có {players.Length} người tham gia";
        string setList = string.Empty;
        foreach (Player p in players) setList += p.Name + "          ";

        PlayerList.text = setList;
    }

    /// <summary>
    /// Sự kiện click của nút yêu cầu vào phòng
    /// </summary>
    public void EventRequestToEnterClick()
    {
        Player.GetOwner().Name = StringHandler.RemoveNonPrintChars(InputPlayerName.text);

        if (StringHandler.RemoveNonPrintChars(Password.text) == SceneManager.RoomPassword) RequestFlag = true;
        else RequestFlag = false;
    }

    /// <summary>
    /// Sự kiện thay đổi giá trị của trường nhập mật khẩu
    /// </summary>
    public void EventRoomPasswordInputFieldTextChangedHandler()
    {
        RequestFlag = false;
    }

    /// <summary>
    /// Sự kiện click của nút máy chủ bắt đầu game
    /// </summary>
    public void EventHostStartGameClick()
    {
        SceneManager.EnterGame = true;
        Scene.WaitingRoomToPlay();
    }

    public void SetActive(bool active)
    {
        DisplayControlsOfServerOrClient();
        gameObject.SetActive(active);
    }

    /// <summary>
    /// Xây dựng trang giao diện nếu như là máy chủ sử dụng controls của máy chủ, ngược lại
    /// </summary>
    public void DisplayControlsOfServerOrClient()
    {
        ServerControls.gameObject.SetActive(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost);
        ClientControls.gameObject.SetActive(SceneManager.IsClient);
    }
}
