using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_PCI : TileObject_PCI
{
    public override void OnInteracted(Player_HJH player)
    {
        if(player.Keys > 2)
        {
            BackendManager.Instance.winUser = player.PlayerName.text;

            BackendManager.Instance.isEscapeWin = true;

            foreach (var userData in BackendManager.Instance.inGameUserList)
            {
                // �̹� �й��� ������� ����Ʈ�� ���� ����
                if(!BackendManager.Instance.userGradeList.Contains(userData.Value))
                    BackendManager.Instance.userGradeList.Add(userData.Value);
            }
            
            Debug.Log(BackendManager.Instance.userGradeList.Count + "���� �¸��� ����Ʈ ũ��Ȯ��");
            BackendManager.Instance.SendResultToServer();
        }
    }
}
