using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputQuestionFields : MonoBehaviour
{
    [SerializeField] private ScrollQuestionView View;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField Id;
    [SerializeField] private TMP_InputField Question;
    [SerializeField] private TMP_InputField Answer;
    [SerializeField] private TMP_InputField Choice1;
    [SerializeField] private TMP_InputField Choice2;
    [SerializeField] private TMP_InputField Choice3;

    public void FillInputFields(string id, string question, string answer, string choice1, string choice2, string choice3)
    {
        Id.text = id;
        Question.text = question;
        Answer.text = answer;
        Choice1.text = choice1;
        Choice2.text = choice2;
        Choice3.text = choice3;
    }

    public void FillClick()
    {
        if (View.SelectedField != null)
        {
            string[] values = View.SelectedField.GetValues();
            FillInputFields(values[0], values[1], values[2], values[3], values[4], values[5]);
        }
    }

    public void ClearClick()
    {
        Id.text = string.Empty;
        Question.text = string.Empty;
        Answer.text = string.Empty;
        Choice1.text = string.Empty;
        Choice2.text = string.Empty;
        Choice3.text = string.Empty;
    }

    public void AddClick()
    {

    }

    public void RemoveClick()
    {

    }

    public void ReplaceClick()
    {

    }
}
