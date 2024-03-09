using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestionField : MonoBehaviour
{
    [SerializeField] private TMP_InputField Id;
    [SerializeField] private TMP_InputField Question;
    [SerializeField] private TMP_InputField Answer;
    [SerializeField] private TMP_InputField Choice1;
    [SerializeField] private TMP_InputField Choice2;
    [SerializeField] private TMP_InputField Choice3;

    [SerializeField] private QuestionsManage Manager;

    public void Remove()
    {
        Manager.QuestionFields.Remove(this);
        Destroy(this);
    }
}
