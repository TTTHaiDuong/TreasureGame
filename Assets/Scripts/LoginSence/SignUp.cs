using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SignUp : MonoBehaviour
{
    [SerializeField] private Login Login;
    [SerializeField] private TMP_InputField EmailField;
    [SerializeField] private TMP_InputField PasswordField1;
    [SerializeField] private TMP_InputField PasswordFiedl2;

    public void Show()
    {
        Login.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
