using System.Collections;
using GameUI;
using TreasureGame;
using Unity.VisualScripting;
using UnityEngine;
using GameItems;
using UnityEditor;
using UnityEngine.UIElements;
using Unity.Netcode;

public class Player : MonoBehaviour
{
    [SerializeField] private int _Gold;
    [SerializeField] private int _Score;
    [SerializeField] private string _Name;
    [SerializeField] private Animator Animator; //Add

    public static Player Main;

    public FlyText NameTable; //Add
    public Flag Flag; //Add
    public ItemList Bag;
    public ReadMap ReadMap; //
    public Island Island; //
    public PlayUI PlayUI;
    public Inventory Inventory;
    public UseItem UseItem;

    public Vector3 TargetPosition;
    public Timer RevivalTimer;
    public int Progress;

    public bool IsMainPlayer;

    public string Name
    {
        set => SetName(value);
        get => GetName();
    }

    public int Gold
    {
        set
        {
            _Gold = value;
            PlayUI.DisplayPlayer(this);
        }
        get => _Gold;
    }

    public int Score
    {
        set
        {
            _Score = value;
            PlayUI.DisplayPlayer(this);
        }
        get => _Score;
    }

    private void Awake()
    {
        if (Main == null) Main = this;

        Bag = new();

        RevivalTimer = gameObject.AddComponent<Timer>();
        //RevivalTimer.WhenStart += Flag.PutUpFlag;
        //RevivalTimer.WhenFinish += Flag.Revival;
        //RevivalTimer.WhenBreak += Flag.Revival;

        PlayUI = GameObject.FindGameObjectWithTag("PlayUI").GetComponent<PlayUI>();
        //Island = GameObject.FindGameObjectWithTag("Island").GetComponent<Island>();
    }

    private void Start()
    {
        NameTable.LookAt(Camera.main.transform.forward);
        Name = "Dương";
    }

    void Update()
    {
        if (Main == null) Main = this;
        // Khong nen de bo dieu khien o day, phai dua ra ngoai de tao game multi-player.
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("TopBlock"))
                MoveTo(hit.collider.bounds.center);
        }
        if (Input.GetKeyDown(KeyCode.Space) && !Animator.GetBool("IsWalking"))
        {
            if (!RevivalTimer.IsRunning) DigUp();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Glasses glasses = new(1);
            glasses.Detect(BlockUnderFoot(), Island.GetComponent<Island>().GetAllBlocks(), 9);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            RevivalTimer.Break();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            Bag.Add(new Glasses(1));
            Bag.Add(new Bomb(1, 200));
        }
    }

    public void GetItems(GameItem[] list)
    {
        foreach (GameItem item in list)
        {
            switch (item.GetType().Name)
            {
                case "Gold": Gold += item.Count; break;
                case "Score": Score += item.Count; break;
                case "TreasureMap": break;
                default: Bag.Add(item); break;
            }
        }
    }

    public void SetName(string name)
    {
        _Name = name;
        NameTable.SetText(name);
    }

    public string GetName() => _Name;

    public void MoveTo(Vector3 targetPosition)
    {
        if (!RevivalTimer.IsRunning)
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
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg - 90;
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

    public void Exploded(float second)
    {
        if (RevivalTimer.IsRunning) RevivalTimer.Time += second;
        else RevivalTimer.Play(second);

        PlayUI.DisplayPlayer(this);
    }

    public void DigUp(Block block)
    {
        block.DugUp(this);
    }

    public void DigUp()
    {
        Block block = BlockUnderFoot();
        DigUp(block);
    }

    public Block BlockUnderFoot()
    {
        return Glasses.DefineAround(transform.position, Island.GetAllBlocks(), 1)[0];
    }
}