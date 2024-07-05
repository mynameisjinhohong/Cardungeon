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
                // 이미 패배한 유저라면 리스트에 넣지 않음
                if(!BackendManager.Instance.userGradeList.Contains(userData.Value))
                    BackendManager.Instance.userGradeList.Add(userData.Value);
            }
            
            Debug.Log(BackendManager.Instance.userGradeList.Count + "열쇠 승리시 리스트 크기확인");
            BackendManager.Instance.SendResultToServer();
        }
    }
}
