using UnityEngine;

public class TopBlock : MonoBehaviour
{
    private Color Origin;
    public Color Highlight;
    public bool IsMouseEnter;

    void Awake()
    {
        Origin = GetComponent<Renderer>().material.color;
    }

    public void OnMouseEnter()
    {
        if (SceneManager.IsClient)
        {
            GetComponent<Renderer>().material.color = Highlight;
            IsMouseEnter = true;
        }
    }

    public void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = Origin;
        IsMouseEnter = false;
    }
}
