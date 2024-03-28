using TMPro;
using Unity.Netcode;
using UnityEngine;

public class WaitingRoom : MonoBehaviour, IUISetActive
{
    [SerializeField] private SceneManager Scenes;
    [SerializeField] private TextMeshProUGUI CountPlayer;
    [SerializeField] private TextMeshProUGUI PlayerList;

    [SerializeField] private RectTransform StartButton;
    [SerializeField] private RectTransform ClientControls;
    [SerializeField] private RectTransform ServerControls;
    [SerializeField] private TMP_InputField Password;

    public TMP_InputField InputPlayerName;
    public bool RequestFlag;

    private void Update()
    {
        Listed();
    }

    private void Listed()
    {
        Player[] players = Player.FindPlayersWithCondition(p => p.IsLoggedIn && p.IsClient && !p.IsHost);

        CountPlayer.text = $"Hiện tại có {players.Length} người tham gia";
        string setList = string.Empty;
        foreach (Player p in players)
            if (p.StudentId != "") setList += p.Name + "     ";

        PlayerList.text = setList;
    }

    public void EventRequestToEnterClick()
    {
        Player.GetOwner().Name = StringHandler.RemoveNonPrintChars(InputPlayerName.text);

        if (StringHandler.RemoveNonPrintChars(Password.text) == SceneManager.RoomPassword) RequestFlag = true;
        else RequestFlag = false;
    }

    public void EventRoomPasswordInputFieldTextChangedHandler()
    {
        RequestFlag = false;
    }

    public void EventHostStartGameClick()
    {
        Scenes.WaitingRoomToPlay();
        SceneManager.EnterGame = true;
    }

    public void SetActive(bool active)
    {
        DisplayControlsOfServerOrClient();
        gameObject.SetActive(active);
    }

    public void DisplayControlsOfServerOrClient()
    {
        ServerControls.gameObject.SetActive(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost);
        ClientControls.gameObject.SetActive(SceneManager.IsClient);
    }
}
