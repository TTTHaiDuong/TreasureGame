using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestionField : MonoBehaviour, IPointerClickHandler
{
    private bool _Selected;

    [SerializeField] private TextMeshProUGUI Id;
    [SerializeField] private TextMeshProUGUI Question;
    [SerializeField] private TextMeshProUGUI Answer;
    [SerializeField] private TextMeshProUGUI Choice1;
    [SerializeField] private TextMeshProUGUI Choice2;
    [SerializeField] private TextMeshProUGUI Choice3;

    private Image ThisImage;

    private readonly Color ColorSelected = new(0f, 121f / 255f, 255f / 255f);
    private readonly Color ColorUnSelected = Color.white;

    public bool Selected
    {
        set { _Selected = value; if (value) Select(); else UnSelect(); }
        get { return _Selected; }
    }

    private void Awake()
    {
        ThisImage = GetComponent<Image>();
    }

    public void SetValues(string id, string question, string answer, string choice1, string choice2, string choice3)
    {
        Id.text = id;
        Question.text = question;
        Answer.text = answer;
        Choice1.text = choice1;
        Choice2.text = choice2;
        Choice3.text = choice3;
    }

    public void Select()
    {
        ThisImage.color = ColorSelected;

    }

    public void UnSelect()
    {
        ThisImage.color = ColorUnSelected;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Selected = !Selected;
    }

    public string[] GetValues()
    {
        string[] result = new string[6];
        result[0] = StringHandler.Simplify(Id.text);
        result[1] = StringHandler.Simplify(Question.text);
        result[2] = StringHandler.Simplify(Answer.text);
        result[3] = StringHandler.Simplify(Choice1.text);
        result[4] = StringHandler.Simplify(Choice2.text);
        result[5] = StringHandler.Simplify(Choice3.text);

        return result;
    }
}
