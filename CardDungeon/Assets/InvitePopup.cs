using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine;

public class InvitePopup : MonoBehaviour
{
    public SessionId InvitedRoomID;
    public string InvitedRoomToken;
    public Action action;
    
    public void AcceptButtonClick()
    {
        Backend.Match.AcceptInvitation(InvitedRoomID, InvitedRoomToken);
        
        //방정보 동기화 해야함
        MatchController.Instance.ChangeUI(2);
    }

    public void RefuseButtonClick()
    {
        UIManager.Instance.PopupListPop();
    }
}
