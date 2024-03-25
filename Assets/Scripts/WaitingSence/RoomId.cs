using TMPro;
using UnityEngine;

public class RoomId : MonoBehaviour
{
    private TMP_InputField InputId;

    private int IdLength = 6;

    public int Id
    {
        get
        {
            if (int.TryParse(StringHandler.Simplify(InputId.text), out int id)) return id;
            else return 0;
        }
    }

    private void Awake()
    {
        InputId = GetComponent<TMP_InputField>();
    }

    public void BeNumber()
    {
        string text = StringHandler.Simplify(InputId.text);

        for (int i = 0; i < text.Length; i++)
            if (!char.IsDigit(text[i])) text = text.Replace(text[i].ToString(), "");

        if (IdLength <= text.Length) text = text.Substring(0, 6);
        InputId.text = text;
    }
}
