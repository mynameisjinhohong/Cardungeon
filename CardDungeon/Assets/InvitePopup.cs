using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using BackEnd.Tcp;
using TMPro;
using UnityEngine;

public class InvitePopup : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    
    public SessionId InvitedRoomID;
    public string InvitedRoomToken;
    public Action action;
    
    public void AcceptButtonClick()
    {
        Backend.Match.AcceptInvitation(InvitedRoomID, InvitedRoomToken);

        Backend.Match.OnMatchMakingRoomUserList = (MatchMakingGamerInfoListInRoomEventArgs args) =>
        {
            int userCount = 0;
            userCount = args.UserInfos.Count;
            Debug.Log(args.UserInfos.Count + "명 조회");
            BackendManager.Instance.roomSettingData.roomHeadCount = args.UserInfos.Count;
            
            BackendManager.Instance.GetMatchList();
            
            MatchController.Instance.FindMatchIndex();

            for (int i = 0; i < userCount; i++)
            {
                Debug.Log(args.UserInfos[i].m_nickName + "님 참가중" + args.UserInfos[i].m_sessionId);
            }
        };
        //방정보 동기화 해야함
        MatchController.Instance.ChangeUI(2);
    }
    public void RefuseButtonClick()
    {
        Backend.Match.DeclineInvitation(InvitedRoomID, InvitedRoomToken);
        UIManager.Instance.PopupListPop();
    }
}
