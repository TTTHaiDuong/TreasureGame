using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameInput : MonoBehaviour
{
    private TMP_InputField Input;
    private int NameLength = 12;

    public string PlayerName { get { return StringHandler.Simplify(Input.text); } }

    private void Awake()
    {
        Input = GetComponent<TMP_InputField>();
    }

    public void TextChange()
    {
        string text = Input.text;
        if (text.Length > NameLength) text = text.Substring(0, NameLength);
        Input.text = text;
    }
}
