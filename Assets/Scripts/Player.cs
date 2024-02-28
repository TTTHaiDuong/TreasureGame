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
    [SerializeField] private string _Name;
    [SerializeField] private Animator Animator;

    public FlyText NameTable;
    public ItemList Bag;
    public Island Island;
    public PlayUI PlayUI;
    public Inventory Inventory;
    public UseItem UseItem;

    public Vector3 TargetPosition;
    public Timer LivingTimer;
    public bool IsActive;

    public string Name
    {
        set => SetName(value);
        get => GetName();
    }

    private void Awake()
    {
        Bag = new();

        LivingTimer = gameObject.AddComponent<Timer>();
        LivingTimer.StartListening(SetActive);
        LivingTimer.FinishListening(SetActive);
        LivingTimer.StartObj = false;
        LivingTimer.FinishObj = true;
        //RevivalTimer.WhenStart += Flag.PutUpFlag;
        //RevivalTimer.WhenFinish += Flag.Revival;
        //RevivalTimer.WhenBreak += Flag.Revival;

        PlayUI.DisplayPlayer(this);
        //Island = GameObject.FindGameObjectWithTag("Island").GetComponent<Island>();
    }

    private void Start()
    {
        NameTable.LookAt(Camera.main.transform.forward);
        Name = "Dương";

        IsActive = true;
    }

    void Update()
    {
        if (IsActive)
        {
            MoveInput(Input.GetMouseButton(0));
            if (Input.GetKeyDown(KeyCode.Space) && !Animator.GetBool("IsWalking"))
            {
                if (!LivingTimer.IsRunning) DigUp();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Glasses glasses = new(1);
                glasses.Detect(BlockUnderFoot(), Island.GetComponent<Island>().GetAllBlocks(), 9);
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                Bag.Add(new Glasses(1));
                Bag.Add(new Bomb(1, 200));
            }
        }
        SkipInput(Input.GetKeyDown(KeyCode.C));
    }

    private void SkipInput(bool keyCaught)
    {
        if (keyCaught) LivingTimer.Time = 0;
    }

    //public void GetItems(GameItem[] list)
    //{
    //    foreach (GameItem item in list)
    //    {
    //        switch (item.GetType().Name)
    //        {
    //            case "Gold": Gold += item.Count; break;
    //            case "Score": Score += item.Count; break;
    //            case "TreasureMap": break;
    //            default: Bag.Add(item); break;
    //        }
    //    }
    //}

    public void SetName(string name)
    {
        _Name = name;
        NameTable.SetText(name);
    }
    public string GetName() => _Name;

    private void MoveInput(bool key)
    {
        if (key)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
        if (LivingTimer.IsRunning) LivingTimer.Time += second;
        else LivingTimer.Play(second);

        PlayUI.DisplayPlayer(this);
    }

    public void SetActive(object active)
    {
        IsActive = (bool)active;
        foreach (Transform partOfBody in transform)
            if (partOfBody.CompareTag("PartOfBody")) partOfBody.gameObject.SetActive((bool)active);
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