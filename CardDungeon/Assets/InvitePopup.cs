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
        MatchController.Instance.uIList[2].GetComponent<UI_Lobby_PCI>().roomNameText.text = BackendManager.Instance.inviterName + "의 방";
        //방정보 동기화 해야함
        MatchController.Instance.ChangeUI(2);
    }
    public void RefuseButtonClick()
    {
        Backend.Match.DeclineInvitation(InvitedRoomID, InvitedRoomToken);
        UIManager.Instance.PopupListPop();
    }
}
