using System.Data;
using TreasureGame;
using UnityEngine;

public class ScrollQuestionView : MonoBehaviour
{
    [SerializeField] private QuestionField FieldSample;
    private RemoteDatabase Database;

    public QuestionField SelectedField { private set; get; }

    private void Start()
    {
        LoadView();
    }

    public void UnSelectOtherFields(QuestionField selected)
    {
        SelectedField = selected;

        QuestionField[] fields = Resources.FindObjectsOfTypeAll<QuestionField>();
        foreach (QuestionField field in fields)
            if (!field.Equals(selected)) field.UnSelect();
    }

    public void LoadView()
    {
        Database = FindObjectOfType<RemoteDatabase>();
        RemoteDatabase.InvokeIfSuccessful(FillGridView);
        Database.ExecuteQuery($"SELECT * FROM {DatabaseManager.QuestionsTableName}");
    }

    public void FillGridView()
    {
        DataTable tb = RemoteDatabase.Result;

        foreach (DataRow row in tb.Rows)
        {
            QuestionField field = Instantiate(FieldSample);
            InitField(field, row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString(), row[5].ToString());
        }
    }

    private void InitField(QuestionField field, string id, string question, string answer, string choice1, string choice2, string choice3)
    {
        field.gameObject.SetActive(true);
        field.SetValues(id, question, answer, choice1, choice2, choice3);
        field.transform.SetParent(transform, true);

        Vector2 pos = new(0, 0);
        field.transform.localPosition = pos;
        foreach (Transform fie in transform)
            if (fie.CompareTag("QuestionField"))
            {
                if (System.Math.Abs(fie.transform.localPosition.y - pos.y) > 0.00001f) break;
                else pos.y -= FieldSample.GetComponent<RectTransform>().sizeDelta.y;
            } 

        field.transform.localPosition = pos;
    }
}
