using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        GetComponent<Renderer>().material.color = Highlight;
        IsMouseEnter = true;
    }

    public void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = Origin;
        IsMouseEnter = false;
    }
}
