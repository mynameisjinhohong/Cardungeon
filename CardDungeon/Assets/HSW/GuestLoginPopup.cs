using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GuestLoginPopup : MonoBehaviour
{
    [SerializeField] InputField nicknameInput;

    public void UpdateNickname()
    {
        if (!checkNicknameUsable())
        {
            UIManager.Instance.OpenRecyclePopup("안내", "사용할 수 없는 닉네임입니다", null);
            return;
        }

        var bro = Backend.BMember.UpdateNickname(nicknameInput.text);

        if (bro.IsSuccess()) {
            Debug.Log("닉네임 변경 : " + bro);
            
            BackendManager.Instance.GetUserInfo();
            
            Backend.BMember.GetUserInfo( ( callback ) =>
            {
                string nickname = callback.GetReturnValuetoJSON()["row"]["nickname"].ToString();
                
                Debug.Log("유저 아이디 조회" + nickname);
            });

            PlayerPrefs.SetInt("LoginWay", 0);
            
            UIManager.Instance.PopupListPop();
            MatchController.Instance.ChangeUI(1);
        }
        else
        {
            Debug.LogError("닉네임 변경 실패 : " + bro);
        }
    }

    bool checkNicknameUsable()
    {
        return true;
    }
}
