using System.Collections.Generic;
using UnityEngine;

public class QuestionsManage : MonoBehaviour
{
    [SerializeField] private QuestionField BaseQuestionField;
    [SerializeField] private RectTransform RectTransform;

    private Vector2 MousePosition;

    public List<QuestionField> QuestionFields;

    private void Update()
    {
        Scroll(Input.GetMouseButton(0) && false);
    }

    private void Scroll(bool keyInput)
    {
        if (keyInput)
        {
            Vector2 pos = RectTransform.position;
        }
    }
}