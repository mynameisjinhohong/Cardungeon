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
                // 열쇠 승리시에는 승리 유저를 먼저 리스트에 넣음
                if(userData.Value.m_nickname == BackendManager.Instance.winUser)
                    BackendManager.Instance.userGradeList.Add(userData.Value);
                    
                // 이미 패배한 유저라면 리스트에 넣지 않고 나머지 모든 유저 리스트에 넣음
                if(!BackendManager.Instance.userGradeList.Contains(userData.Value))
                    BackendManager.Instance.userGradeList.Add(userData.Value);
            }
            
            Debug.Log(BackendManager.Instance.userGradeList.Count + "열쇠 승리시 리스트 크기확인");

            // 나 자신만 종료 요청함
            if (player.isMine)
            {
                // 탈출 성공한 유저가 슈퍼게이머라면 바로 서버로 전송
                if (player.isSuperGamer)
                {
                    BackendManager.Instance.SendResultToServer();
                }
                else
                {
                    // 슈퍼게이머가 아니라면 슈퍼게이머에게 게임 종료 요청
                    GamePlayManager.Instance.SendToSuperGamerEndGame();
                }
            }
        }
    }
}
