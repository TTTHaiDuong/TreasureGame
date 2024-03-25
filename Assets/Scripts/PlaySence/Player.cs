using GameItems;
using GameUI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Data;
using TreasureGame;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private string _Name;
    [SerializeField] private FlyText NameTable; // Bảng tên nhân vật

    public MainCamera PlayerCamera;
    public PartsOfBody Body;
    public Animator Animator;
    public Island Island;

    private Vector3 TargetPosition;
    public Timer LivingTimer;
    public bool IsActive;

    public string StudentId;
    public int Score;

    #region Điều khiển các chức năng khác của game
    public static bool EnterGame;
    #endregion

    #region Điều khiển Database từ xa
    public static event Action ReceiceData;
    public static DataTable Result;
    public static string Query;
    public static string[] Parameters;
    public static string[] Values;
    #endregion

    public string Name
    {
        set => SetName(value);
        get => _Name;
    }

    private void Awake()
    {
        LivingTimer = gameObject.AddComponent<Timer>();
        LivingTimer.StartListening(SetActive);
        LivingTimer.FinishListening(SetActive);
        LivingTimer.StartObj = false;
        LivingTimer.FinishObj = true;

        Island = FindObjectOfType<Island>();
    }

    private void Start()
    {
        PlayerCamera = Camera.main.GetComponent<MainCamera>();

        transform.position = new(6, 0.13f, 0);

        InitReferences();

        NameTable.LookAt(PlayerCamera.transform.forward);
        Name = "";

        IsActive = true;
    }

    void Update()
    {
        if (IsOwner)
        {
            if (IsActive)
            {
                MoveInput(Input.GetMouseButton(0));
                DigUp(Input.GetKeyDown(KeyCode.Space) && !Animator.GetBool("IsWalking"));
                ThrowBomb(Input.GetKeyDown(KeyCode.F));

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    //Xem vị trí bom dưới đất
                }
            }

            Timer time = new GameObject().AddComponent<Timer>();
            if (TimeCountDown.Timer != null) time = TimeCountDown.Timer;
            SyncFeaturesClientRpc(EnterGame, SceneManager.PlayingTime, SceneManager.RoomPassword, time);
            SyncServerRpc(Name, StudentId, Score, transform.position, transform.forward, Animator.GetBool("IsWalking"), IsActive);
            ExecuteSendToServerRpc(Query, new(Parameters), new(Values), new(Result));
        }

        SkipInput(Input.GetKeyDown(KeyCode.C));

        if (IsServer || IsHost) Result = DatabaseManager.ExecuteQuery(Query, Parameters, Values);
    }

    public void InitReferences()
    {
        if (IsOwner)
        {
            GameObject[] initComponents = IInitOwnerComponent.GameObjectsWithThisInterface();
            foreach (GameObject obj in initComponents)
                obj.GetComponent<IInitOwnerComponent>().SetOwner(this);
        }
    }

    /// <summary>
    /// Debug...
    /// </summary>
    private void SkipInput(bool active)
    {
        if (active) LivingTimer.Time = 0;
    }

    /// <summary>
    /// Event Animation tham chiếu đến.
    /// </summary>
    public void FinishThrowing()
    {
        Animator.SetBool("IsThrowing", false);
    }

    public void SetName(string name)
    {
        _Name = name;
        NameTable.SetText(name);
    }

    #region Đồng bộ các thuộc tính nhân vật
    [ServerRpc]
    private void SyncServerRpc(string name, string studentId, int score, Vector3 position, Vector3 forward, bool isWalking, bool isActive)
    {
        PlayerInfo sync = new()
        {
            Name = name,
            StudentId = studentId,
            Score = score,
            Position = position,
            Forward = forward,
            IsWalking = isWalking,
            IsActive = isActive
        };
        SyncClientRpc(sync);
    }

    [ClientRpc]
    private void SyncClientRpc(PlayerInfo sync)
    {
        if (!IsOwner) sync.Deserialize(this);
    }
    #endregion

    #region Di chuyển
    private void MoveInput(bool key)
    {
        if (key)
        {
            Ray ray = PlayerCamera.gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("TopBlock"))
                MoveTo(hit.collider.bounds.center);
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (!LivingTimer.IsRunning)
        {
            TargetPosition = targetPosition;
            if (!Animator.GetBool("IsWalking")) StartCoroutine(Go());
        }
    }

    private IEnumerator Go()
    {
        Animator.SetBool("IsWalking", true);
        while (true)
        {
            Vector3 direction = (TargetPosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion lookRotation = Quaternion.Euler(0f, angle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 20);

            Vector3 newPos = new(TargetPosition.x, transform.position.y, TargetPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime * 5);
            if (Vector3.Distance(newPos, transform.position) <= 0.01)
            {
                Animator.SetBool("IsWalking", false);
                transform.position = newPos;
                break;
            }
            yield return null;
        }
    }
    #endregion

    #region Thảy bom
    public void ThrowBomb(bool active)
    {
        if (active)
        {
            Animator.SetBool("IsThrowing", true);
            ExplosionThrownBomb bomb =
                Instantiate(GameObject.FindGameObjectWithTag("ExplosionThrownBomb").GetComponent<ExplosionThrownBomb>());

            bomb.Throw(this, transform.forward);

            if (IsOwner) ThrowBombServerRpc();
        }
    }

    [ServerRpc]
    private void ThrowBombServerRpc()
    {
        ThrowBombClientRpc();
    }

    [ClientRpc]
    private void ThrowBombClientRpc()
    {
        if (!IsOwner) ThrowBomb(true);
    }
    #endregion

    #region Đồng bộ các tính năng của trò chơi
    public static void SetFeaturesGame(bool enterGame, int playingTime, string roomPassword, Timer timer)
    {
        GetOwner().SyncFeaturesServerRpc(enterGame, playingTime, roomPassword, timer);
    }

    [ServerRpc]
    private void SyncFeaturesServerRpc(bool enterGame, int playingTime, string roomPassword, Timer timer)
    {
        SyncFeaturesClientRpc(enterGame, playingTime, roomPassword, timer);
    }

    [ClientRpc]
    private void SyncFeaturesClientRpc(bool enterGame, int playingTime, string roomPassword, Timer timer)
    {
        EnterGame = enterGame;
        SceneManager.PlayingTime = playingTime;
        SceneManager.RoomPassword = roomPassword;
        timer.NetworkDeserialize(TimeCountDown.Timer);
    }
    #endregion

    public void Exploded(float second)
    {
        if (LivingTimer.IsRunning) LivingTimer.Time += second;
        else LivingTimer.Play(second);
    }

    public void SetActive(object active)
    {
        IsActive = (bool)active;
        Body.SetActiveChildren(IsActive);
    }

    public void DigUp(bool active)
    {
        if (active)
        {
            Block block = BlockUnderFoot();
            block.DugUp(this);
        }
    }

    public Block BlockUnderFoot()
    {
        return Glasses.DefineAround(transform.position, Island.GetAllBlocks(), 1)[0];
    }

    #region Điều khiển từ xa Database
    public static void ExecuteQuery(string query, string[] parameters, string[] values)
    {
        Query = query;
        Parameters = parameters;
        Values = values;
    }

    [ServerRpc]
    private void ExecuteSendToServerRpc(string query, ArraySerializable<string> paraS, ArraySerializable<string> valS, DataTableSerializable table)
    {
        ReceiveClientRpc(query, paraS, valS, table);
    }

    [ClientRpc]
    private void ReceiveClientRpc(string query, ArraySerializable<string> paraS, ArraySerializable<string> valS, DataTableSerializable table)
    {
        Result = table.Deserialize();
        Query = query;
        Parameters = paraS.Deserialize();
        Values = valS.Deserialize();
        ReceiceData?.Invoke();
    }
    #endregion

    public static Player GetOwner()
    {
        Player[] players = FindObjectsOfType<Player>();
        foreach (Player player in players)
            if (player.IsOwner) return player;
        return null;
    }
}

public class PlayerInfo : INetworkSerializable
{
    public string Name;
    public string StudentId;
    public int Score;
    public Vector3 Position;
    public Vector3 Forward;
    public bool IsWalking;
    public bool IsActive;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref StudentId);
        serializer.SerializeValue(ref Score);
        serializer.SerializeValue(ref Position);
        serializer.SerializeValue(ref Forward);
        serializer.SerializeValue(ref IsWalking);
        serializer.SerializeValue(ref IsActive);
    }

    public void Deserialize(Player player)
    {
        player.SetName(Name);
        player.StudentId = StudentId;
        player.Score = Score;
        player.transform.position = Position;
        player.transform.forward = Forward;
        player.Animator.SetBool("IsWalking", IsWalking);
        player.IsActive = IsActive;
    }
}