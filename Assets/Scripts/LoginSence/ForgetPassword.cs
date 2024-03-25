using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ForgetPassword : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private PasswordRecover PasswordRecover;
    [SerializeField] private RectTransform Login;
    [SerializeField] private RectTransform SignUp;
    [SerializeField] private TextMeshProUGUI Text;

    private string OriginalText;

    private void Awake()
    {
        OriginalText = Text.text;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Text.text = OriginalText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!(Text.text.Contains("<u>") && Text.text.Contains("</u>")))
            Text.text = "<u>" + OriginalText + "</u>";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Login.gameObject.SetActive(false);
        SignUp.gameObject.SetActive(false);
        PasswordRecover.Show();
    }
}
