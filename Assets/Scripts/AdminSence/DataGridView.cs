using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGridView : MonoBehaviour
{
    [SerializeField] private QuestionField Field;

    public float FieldsDistance;

    void Update()
    {
        
    }

    private float FieldViewHeight()
    {
        RectTransform rect = Field.GetComponent<RectTransform>();
        return rect.sizeDelta.y;
    }

    private float UpperPoint()
    {
        float y = 0;
        foreach (Transform dataField in transform)
            if (dataField.CompareTag("DataField"))
            {
                
            }
        return y;
    }

    private float LowerPoint()
    {
        float y = 0;
        return y;
    }
}
