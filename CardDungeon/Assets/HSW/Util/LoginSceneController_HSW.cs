using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneController_HSW : MonoBehaviour
{
    public TMP_InputField NickNameText;

    public void NickNameChanged()
    {
        string name = NickNameText.text;
        
        BackendReturnObject bro = Backend.BMember.UpdateNickname(name);
        BackendManager.Instance.Nickname = name;
    }
}
