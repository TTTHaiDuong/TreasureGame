using UnityEngine;

public class MainCamera : MonoBehaviour, IInitOwnerComponent
{
    private Vector3 StartMousePosition;
    private Vector3 DragStartCameraPosition;
    private Vector3 OriginalForward;
    private bool IsCameraDragging;
    private readonly Vector3 ConstDelta = new(13.6f, -20.27f, 0.05f);

    public Vector3 DeltaPosition;

    public Player Player { private set; get; }

    public void Init()
    {
        OriginalForward = transform.forward;
        DeltaPosition = Player.transform.position - transform.position;
    }

    public void SetStandardForward()
    {
        DeltaPosition = ConstDelta;
    }

    private void Update()
    {
        if (Player != null)
        {
            transform.forward = OriginalForward;
            if (!IsCameraDragging) transform.position = Player.transform.position - DeltaPosition;
        }
    }

    public void Peek(bool active)
    {
        if (active) Drag();
        else if (Input.GetMouseButtonUp(1)) IsCameraDragging = false;

    }

    public void Move(bool active)
    {
        if (active)
        {
            Drag();
        }
        else if (Input.GetMouseButtonUp(0)) 
        {
            DeltaPosition = Player.transform.position - transform.position;
            IsCameraDragging = false;
        }
    }

    private void Drag()
    {
        if (!IsCameraDragging)
        {
            DragStartCameraPosition = transform.position;
            StartMousePosition = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y);
            IsCameraDragging = true;
        }
        Vector3 dragDelta = transform.TransformDirection(new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y) - StartMousePosition);
        dragDelta.x *= 2.5f;
        dragDelta.y = 0;
        transform.position = DragStartCameraPosition - 0.02f * dragDelta;
    }

    public void SetOwner(Player player)
    {
        Player = player;
        Player.PlayerCamera = this;
        Init();
    }

    public void RemoveOwner(Player player)
    {
        Player.PlayerCamera = null;
        Player = null;
    }
}
