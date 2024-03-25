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

    private void Init()
    {
        OriginalForward = transform.forward;
        DeltaPosition = Player.transform.position - transform.position;
    }

    private void Update()
    {
        if (Player != null)
        {
            transform.forward = OriginalForward;
            if (!IsCameraDragging) transform.position = Player.transform.position - DeltaPosition;

            Dragging(Input.GetMouseButton(1));
        }
    }

    private void Dragging(bool active)
    {
        if (active)
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
        else if (Input.GetMouseButtonUp(1))
        {
            IsCameraDragging = false;
            transform.position = DragStartCameraPosition;
        }
    }

    public void SetStandardPosition()
    {
        if (Player != null)
            transform.position = Player.transform.position - ConstDelta;
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
