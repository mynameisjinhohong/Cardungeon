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
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RefuseButtonClick();
        }
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AcceptButtonClick();
        }
    }
    
    public void AcceptButtonClick()
    {
        Backend.Match.AcceptInvitation(InvitedRoomID, InvitedRoomToken);
        Debug.Log(InvitedRoomToken + "구분" + InvitedRoomID);
        MatchController.instance.roomNameText.text = BackendManager.Instance.inviterName + "의 방";
        MatchController.instance.btn_Invite.interactable = false;
        MatchController.instance.btn_MatchStart.interactable = false;
        //방정보 동기화 해야함
        MatchController.instance.ChangeUI(2);
    }
    public void RefuseButtonClick()
    {
        Backend.Match.DeclineInvitation(InvitedRoomID, InvitedRoomToken);
        UIManager.Instance.PopupListPop();
    }
}
