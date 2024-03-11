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
        Debug.Log("초대 보내기 시도");
        Backend.Match.InviteUser(nickNameInput.text);
        
        Backend.Match.OnMatchMakingRoomInvite = (MatchMakingInteractionEventArgs args) => {
            Debug.Log("에러인포 :" + args.ErrInfo + "이유:" + args.Reason);

            if (args.ErrInfo == ErrorCode.Success)
            {
                UIManager.Instance.OpenRecyclePopup("초대 결과", "초대 요청에 성공 했습니다.", UIManager.Instance.PopupListPop);
            }
            else
                UIManager.Instance.OpenRecyclePopup("초대 결과", args.Reason, null);
        };
    }
}
