using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Mail;
using System;
using TMPro;

public class PasswordRecover : MonoBehaviour
{
    [SerializeField] private TMP_InputField EmailField;
    [SerializeField] private TextMeshProUGUI Notification;

    private Timer Timer;

    private void Awake()
    {
        Timer = new GameObject().AddComponent<Timer>();
        Timer.StartListening((obj) => { });
        Timer.FinishListening((obj) => { });
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void ClearNotification()
    {
        Notification.text = string.Empty;
    }

    public void Send()
    {
        string fromEmail = "";
        string password = "";

        string toEmail = StringHandler.Simplify(EmailField.text);

        System.Random rand = new();
        int otp = rand.Next(100000, 999999);

        string subject = "Khôi phục mật khẩu";
        string body = $"Mật khẩu mới của bạn là: {otp}";

        Notification.text = "Hãy kiểm tra email của bạn!";
        try
        {
            MailMessage message = new(fromEmail, toEmail, subject, body);
            SmtpClient client = new("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, password)
            };

            client.Send(message);
        }
        catch (Exception)
        {
            Notification.text = "Gửi thất bại!";
        }
    }
}
