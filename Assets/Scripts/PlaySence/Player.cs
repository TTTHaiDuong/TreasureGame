using GameItems;
using GameUI;
using System;
using System.Collections;
using System.Data;
using System.Linq;
using TreasureGame;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    #region Unity objects
    [SerializeField] private string _Name;
    [SerializeField] private FlyText NameTable;

    [HideInInspector] public Inventory Inventory;
    [HideInInspector] public MainCamera PlayerCamera;
    [HideInInspector] public Island Island;

    public PartsOfBody Body;
    public Animator Animator;

    private Vector3 TargetPosition;
    public Timer LivingTimer;
    #endregion

    public string StudentId;
    public int Score;

    #region Các chức năng
    public bool IsActive;
    public bool IsLoggedIn;
    public bool IsEnterGame;
    public bool IsVisible { private set; get; } = true;
    #endregion

    #region Điều khiển Database từ xa
    public static event Action ReceiceData;
    private static DataTable _Result;
    public static DataTable Result
    {
        set { _Result = value; }
        get { if (_Result != null) return _Result.Clone(); else return null; }
    }
    #endregion

    public string Name
    {
        set => SetName(value);
        get => _Name;
    }

    private void Awake()
    {
        PlayerCamera = Camera.main.GetComponent<MainCamera>();

        LivingTimer = gameObject.AddComponent<Timer>();
        LivingTimer.StartListening((obj) => SetActive(false));
        LivingTimer.FinishListening((obj) => SetActive(true));

        Island = FindObjectOfType<Island>();
    }

    private void Start()
    {
        InitReferences();
        NameTable.LookAt(PlayerCamera.transform.forward);

        Island.InitIsland();
    }

    void Update()
    {
        if (IsOwner)
        {
            #region other code
            if (!SceneManager.IsClient)
                PlayerCamera.Move(Input.GetMouseButton(0));

            if (IsActive && IsEnterGame && SceneManager.IsClient)
            {
                MoveInput(Input.GetMouseButton(0));
                DigUp(Input.GetKeyDown(KeyCode.Space) && !Animator.GetBool("IsWalking"));
                ThrowBomb(Input.GetKeyDown(KeyCode.F));

                Inventory.Glasses.Use(Input.GetKeyDown(KeyCode.Alpha1));
                Inventory.Shovel.Use(Input.GetKeyDown(KeyCode.Alpha2));
                Inventory.Bomb.Use(Input.GetKeyDown(KeyCode.Alpha3));
            }

            PlayerCamera.Peek(Input.GetMouseButton(1));

            if (SceneManager.RoomPassword != null && TimeCountDown.Timer != null)
                SyncFeaturesServerRpc(IsLoggedIn, SceneManager.EnterGame, IsEnterGame, SceneManager.PlayingTime, SceneManager.RoomPassword, TimeCountDown.Timer.Serialize());
            #endregion
            SyncServerRpc(Name, StudentId, Score, transform.position, transform.forward, Animator.GetBool("IsWalking"));

        }

        if (Result != null)
            foreach (DataColumn col in Result.Columns) Debug.Log(col.ColumnName);

        // Debug...
        SkipInput(Input.GetKeyDown(KeyCode.C));
    }

    public void RandomPositionSpawn()
    {
        float baseY = 0.13f;
        Block[] blocks = Island.GetAllBlocks();
        Block random = new GameRandom().ChooseFromList(blocks);
        Vector3 pos = random.TopBlock.transform.position;
        pos.y = baseY;

        transform.position = pos;
        PlayerCamera.SetStandardForward();
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

    public void Visible(bool visible)
    {
        if (visible == IsVisible) return;
        IsVisible = visible;
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        CanvasRenderer[] cvRenderers = GetComponentsInChildren<CanvasRenderer>();

        foreach (MeshRenderer renderer in renderers) renderer.enabled = visible;
        foreach (CanvasRenderer renderer in cvRenderers) renderer.cullTransparentMesh = visible;
    }

    /// <summary>
    /// Debug...
    /// </summary>
    private void SkipInput(bool active)
    {
        if (active) LivingTimer.Time = 0;
    }

    /// <summary>
    /// Event Animation của nhân vật tham chiếu đến.
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
    private void SyncServerRpc(string name, string studentId, int score, Vector3 position, Vector3 forward, bool isWalking)
    {
        SyncClientRpc(name, studentId, score, position, forward, isWalking);
    }

    [ClientRpc]
    private void SyncClientRpc(string name, string studentId, int score, Vector3 position, Vector3 forward, bool isWalking)
    {
        if (!IsOwner)
        {
            SetName(name);
            StudentId = studentId;
            Score = score;
            transform.position = position;
            transform.forward = forward;
            Animator.SetBool("IsWalking", isWalking);
        }
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
    [ServerRpc]
    private void SyncFeaturesServerRpc(bool isLoggedIn, bool enterGame, bool isEnterGame, int playingTime, string roomPassword, string timerS)
    {
        SyncFeaturesToAllClientRpc(isLoggedIn, enterGame, isEnterGame, playingTime, roomPassword, timerS);
    }

    [ClientRpc]
    private void SyncFeaturesToAllClientRpc(bool isLoggedIn, bool enterGame, bool isEnterGame, int playingTime, string roomPassword, string timerS)
    {
        if (!IsOwner)
        {
            IsLoggedIn = isLoggedIn;
            IsEnterGame = isEnterGame;
            if (!NetworkManager.Singleton.IsServer)
            {
                SceneManager.EnterGame = enterGame;
                SceneManager.PlayingTime = playingTime;
                SceneManager.RoomPassword = roomPassword;
                TimeCountDown.Timer.Deserialize(timerS);
            }
        }
    }
    #endregion

    public void Exploded(float second)
    {
        if (LivingTimer.IsRunning) LivingTimer.Time += second;
        else LivingTimer.Play(second);
    }

    public void SetActive(bool active)
    {
        IsActive = active;
        Body.SetActiveChildren(active);
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

    #region Điều khiển Database từ xa
    public static void ExecuteQuery(string query, string[] parameters, string[] values)
    {
        if (query != "")
        {
            if (NetworkManager.Singleton.IsServer) Result = DatabaseManager.ExecuteQuery(query, parameters, values);
            else if (GetOwner() != null) GetOwner().SendToServerRpc(query, new(parameters), new(values));
        }
    }

    [ServerRpc]
    private void SendToServerRpc(string query, ArraySerializable<string> paraS, ArraySerializable<string> valS)
    {
        Debug.Log("Send Query");
        ExecuteOnServerClientRpc(query, paraS, valS);
    }

    [ClientRpc]
    private void ExecuteOnServerClientRpc(string query, ArraySerializable<string> paraS, ArraySerializable<string> valS)
    {
        Debug.Log("Execute On Server");
        for (int i = 0; i < 20; i++)
        {
            Result = DatabaseManager.ExecuteQuery(query, paraS.Deserialize(), valS.Deserialize());
            if (Result != null && Result.Rows.Count != 0) break;
        }
        if (Result != null && Result.Rows.Count != 0) GetOwner().SendResultToServerRpc(new(Result));
    }

    [ServerRpc]
    private void SendResultToServerRpc(DataTableSerializable table)
    {
        Debug.Log("Send Result");
        ReceiveResultClientRpc(table);
    }

    [ClientRpc]
    private void ReceiveResultClientRpc(DataTableSerializable table)
    {
        if (!IsOwner)
        {
            Debug.Log("Receiver Result");

            Result = table.Deserialize();
            ReceiceData?.Invoke();

            if (Result != null)
                foreach (DataColumn col in Result.Columns) Debug.Log("Client" + col.ColumnName);
        }
    }
    #endregion

    public static Player GetOwner()
    {
        Player[] found = FindPlayersWithCondition(p => p.IsOwner);
        if (found == null || found.Length == 0) return null;
        else return found[0];
    }

    public static Player[] FindPlayersWithCondition(Func<Player, bool> condition)
    {
        Player[] players = FindObjectsOfType<Player>();
        return players.Where(condition).ToArray();
    }
}