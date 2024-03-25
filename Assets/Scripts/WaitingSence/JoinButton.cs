using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinButton : MonoBehaviour
{
    [SerializeField] private PlayerNameInput PlayerName;
    [SerializeField] private RoomId RoomId;

    public void Enter()
    {
        if (PlayerName.PlayerName == string.Empty)
        {
            MessageBox message = MessageBox.CreateMessageBox();
            message.Show(transform.parent.GetComponent<RectTransform>(), "Thông báo!", "Bạn phải nhập tên cho nhân vật.", MessageBoxButton.OK, MessageBoxIcon.ExclamationMask);
            return;
        } 
    }
}
