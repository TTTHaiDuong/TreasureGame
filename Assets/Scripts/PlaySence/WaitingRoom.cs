using GameUI;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class WaitingRoom : MonoBehaviour
{
    [SerializeField] private SceneManager Scenes;
    [SerializeField] private TMP_InputField InputPlayerName;
    [SerializeField] private TextMeshProUGUI CountPlayer;
    [SerializeField] private TextMeshProUGUI PlayerList;

    [SerializeField] private RectTransform StartButton;
    [SerializeField] private RectTransform Client;
    [SerializeField] private RectTransform Server;
    [SerializeField] private RoomIdField RoomId;

    public Player Owner;


    private void Update()
    {
        Listed();
        if ((SceneManager.IsServer || (Player.GetOwner() != null && Player.GetOwner().Name != "")) && Player.EnterGame &&
            (!SceneManager.IsClient || (SceneManager.IsClient && StringHandler.RemoveNonPrintChars(RoomId.GetComponent<TMP_InputField>().text) == SceneManager.RoomPassword)))
        {
            Scenes.WaitingRoomToPlay();
        }
    }

    private void Listed()
    {
        Player[] players = FindObjectsOfType<Player>();

        CountPlayer.text = $"Hiện tại có {players.Length} người tham gia";
        string setList = string.Empty;
        foreach (Player p in players)
            if (p.Name != "") setList += p.Name + "     ";

        PlayerList.text = setList;
    }

    public void SetName()
    {
        Player player = Player.GetOwner();
        player.Name = StringHandler.RemoveNonPrintChars(InputPlayerName.text);
    }

    public void SetActive(bool active)
    {
        IfServerOrClient();
        Player.EnterGame = false;
        gameObject.SetActive(active);
    }

    public void IfServerOrClient()
    {
        Server.gameObject.SetActive(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost);
        RoomId.gameObject.SetActive(SceneManager.IsClient);
        if (Player.GetOwner() == null)
        {
            Client.gameObject.SetActive(false);
            Vector2 posStart = StartButton.transform.position;
            posStart.x = 0;
            StartButton.transform.position = posStart;
        }
    }

    public void SendStartToPlayServerRpc(bool active)
    {
        Player.EnterGame = active;
        Player.SetFeaturesGame(Player.EnterGame, SceneManager.PlayingTime, SceneManager.RoomPassword, TimeCountDown.Timer);
        
        Inventory inv = FindObjectOfType<Inventory>();
        inv.StartGame();

        if (Player.GetOwner() != null) Player.GetOwner().IsActive = true;
    }
}
