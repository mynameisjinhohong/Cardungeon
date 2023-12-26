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
        if (BackendManager.Instance.UserIndate == "")
        {
            BackendManager.Instance.GuestLoginSequense();
        }
        
        var bro = Backend.BMember.UpdateNickname(nicknameInput.text);

        if (bro.IsSuccess()) {
            Debug.Log("닉네임 변경 : " + bro);
            
            BackendManager.Instance.GetUserInfo();
            
            MatchController.Instance.GuestLoginSuccess();
            
            PlayerPrefs.SetInt("LoginWay", 0);
            
        } else {
            Debug.LogError("닉네임 변경 : " + bro);
        }
    }
}
