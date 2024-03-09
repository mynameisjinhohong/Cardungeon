using System.Collections;
using System.Collections.Generic;
using BackEnd;
using BackEnd.Tcp;
using TMPro;
using UnityEngine;

public class NickNameInputToInvite : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField nickNameInput;
    
    public void BackButtonClick()
    {
        UIManager.Instance.PopupListPop();
    }

    public void TryToInvite()
    {
        Debug.Log("�ʴ� ������ �õ�");
        Backend.Match.InviteUser(nickNameInput.text);
        
        Backend.Match.OnMatchMakingRoomInvite = (MatchMakingInteractionEventArgs args) => {
            Debug.Log("�������� :" + args.ErrInfo + "����:" + args.Reason);
        };
    }
}