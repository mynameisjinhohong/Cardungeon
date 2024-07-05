using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class NickNameInputPopup : MonoBehaviour
{
    [SerializeField] InputField nicknameInput;

    public void TryNicknameUpdate()
    {
        if (!checkNicknameUsable())
        {
            UIManager.Instance.OpenRecyclePopup("안내", "사용할 수 없는 닉네임입니다", null);
            return;
        }

        var bro = Backend.BMember.UpdateNickname(nicknameInput.text);
            
        if (bro.IsSuccess()) {
            BackendManager.Instance.GetUserInfo();
        }
        else
        {
            // 닉네임 생성실패 예외처리
        }
    }

    bool checkNicknameUsable()
    {
        if (nicknameInput.text.Length >= 8)
        {
            UIManager.Instance.OpenRecyclePopup("안내", "닉네임은 최대 7글자까지 가능합니다.", null);
            return false;
        }
        
        return true;
    }
}
