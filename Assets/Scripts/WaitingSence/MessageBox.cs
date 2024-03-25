using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    [SerializeField] private Image Icon;
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI Content;
    [SerializeField] private RectTransform OKCancel;
    [SerializeField] private RectTransform OK;
    [SerializeField] private Sprite[] Icons;

    //private readonly SemaphoreSlim Semaphore = new(0);
    private MessageBoxResult Result;

    public static MessageBox CreateMessageBox()
    {
        return Instantiate(Resources.FindObjectsOfTypeAll<MessageBox>()[0]);
    }

    public MessageBoxResult Show(RectTransform parent, string title, string content, MessageBoxButton button)
    {
        ShowP(parent, title, content, button);

        //Semaphore.Wait();
        return Result;
    }

    public MessageBoxResult Show(RectTransform parent, string title, string content, MessageBoxButton button, MessageBoxIcon icon)
    {
        ShowP(parent, title, content, button);

        Icon.gameObject.SetActive(true);
        Icon.sprite = Icons[(int)icon];

        //Semaphore.Wait();
        return Result;
    }

    private void ShowP(RectTransform parent, string title, string content, MessageBoxButton button)
    {
        gameObject.SetActive(true);
        Title.text = title;
        Content.text = content;

        SetButton(button);
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
    }

    private void SetButton(MessageBoxButton button)
    {
        if (button == MessageBoxButton.OKCancel) OKCancel.gameObject.SetActive(true);
        else if (button == MessageBoxButton.OK) OK.gameObject.SetActive(true); 
    }

    public void Close(Button button)
    {
        if (button.CompareTag("OKButton")) Result = MessageBoxResult.OK;
        else if (button.CompareTag("CancelButton")) Result = MessageBoxResult.Cancel;

        //Semaphore.Release();
        Destroy(gameObject);
    }
}

public enum MessageBoxIcon
{
    MultiplyMask,
    QuestionMask,
    ExclamationMask
}

public enum MessageBoxButton
{
    OKCancel,
    OK
}

public enum MessageBoxResult
{
    OK,
    Cancel
}