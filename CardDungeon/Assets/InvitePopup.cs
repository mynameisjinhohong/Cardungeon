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
        
        Backend.Match.OnMatchMakingRoomJoin = (MatchMakingGamerInfoInRoomEventArgs args) =>
        {
            Debug.Log("��Ī�� ����");
            Debug.Log(args.ErrInfo + args.Reason);
        };

        Backend.Match.OnMatchMakingRoomUserList = (MatchMakingGamerInfoListInRoomEventArgs args) =>
        {
            int userCount = 0;
            userCount = args.UserInfos.Count;
            Debug.Log(args.UserInfos.Count + "�� ��ȸ");

            for (int i = 0; i < userCount; i++)
            {
                Debug.Log(args.UserInfos[i].m_nickName + "�� ������" + args.UserInfos[i].m_sessionId);
            }
        };
        //������ ����ȭ �ؾ���
        MatchController.Instance.ChangeUI(2);
    }

    public void RefuseButtonClick()
    {
        Backend.Match.DeclineInvitation(InvitedRoomID, InvitedRoomToken);
        UIManager.Instance.PopupListPop();
    }
}
