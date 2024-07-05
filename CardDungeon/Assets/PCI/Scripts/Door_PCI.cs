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
                // ���� �¸��ÿ��� �¸� ������ ���� ����Ʈ�� ����
                if(userData.Value.m_nickname == BackendManager.Instance.winUser)
                    BackendManager.Instance.userGradeList.Add(userData.Value);
                    
                // �̹� �й��� ������� ����Ʈ�� ���� �ʰ� ������ ��� ���� ����Ʈ�� ����
                if(!BackendManager.Instance.userGradeList.Contains(userData.Value))
                    BackendManager.Instance.userGradeList.Add(userData.Value);
            }
            
            Debug.Log(BackendManager.Instance.userGradeList.Count + "���� �¸��� ����Ʈ ũ��Ȯ��");

            // �� �ڽŸ� ���� ��û��
            if (player.isMine)
            {
                // Ż�� ������ ������ ���۰��̸Ӷ�� �ٷ� ������ ����
                if (player.isSuperGamer)
                {
                    BackendManager.Instance.SendResultToServer();
                }
                else
                {
                    // ���۰��̸Ӱ� �ƴ϶�� ���۰��̸ӿ��� ���� ���� ��û
                    GamePlayManager.Instance.SendToSuperGamerEndGame();
                }
            }
        }
    }
}
