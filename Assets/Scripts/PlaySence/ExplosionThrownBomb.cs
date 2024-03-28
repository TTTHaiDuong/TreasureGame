using GameItems;
using UnityEngine;
using UnityEngine.UI;

public class ExplosionThrownBomb : MonoBehaviour
{
    private Rigidbody Rigidbody;

    [SerializeField] private ThrownBomb ThrownBomb;
    [SerializeField] private Explosion Explosion;
    [SerializeField] private Image Image;

    [Header("Đường bay của Bom ném")]
    public float ExplosionRadius;
    public float Distance;
    public float Speed;
    public Bomb Bomb;

    [HideInInspector] public bool WasExploded;

    public Player Owner { get; private set; }

    private void Awake()
    {
        Bomb = new(1, 20);
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Explosion.Timer.FinishListening((obj) => Destroy(gameObject));
    }

    private void Update()
    {
        transform.rotation = new Quaternion(0, transform.rotation.y, 0, 0);
    }

    public void InfluencePlayers()
    {
        WasExploded = true;
        Rigidbody.velocity = Vector3.zero;
    }

    private void SetUpDirection(Vector3 direction)
    {
        Vector3 pos = Owner.transform.position;
        pos.y = 3;
        transform.position = pos;
        Vector3 velocity = direction.normalized * Speed;
        velocity.y = 0;
        Rigidbody.velocity = velocity;
    }

    public void Throw(Player owner, Vector3 direction)
    {
        Owner = owner;

        gameObject.SetActive(true);
        SetUpDirection(direction);
        Destroy(ThrownBomb.gameObject, Distance / Speed);
    }
}
