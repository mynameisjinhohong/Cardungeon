using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneController_HSW : MonoBehaviour
{
    public TMP_InputField NickNameText;

    public List<GameObject> LoginUIList;

    public void NickNameChanged()
    {
        string name = NickNameText.text;
        
        BackendReturnObject bro = Backend.BMember.UpdateNickname(name);
        BackendManager.Instance.userInfo.Nickname = name;
    }

    public void ChangeHostingUI(int index)
    {
        if (BackendManager.Instance.LoadServerTime) return;

        for (int i = 0; i < LoginUIList.Count; i++)
        {
            LoginUIList[i].gameObject.SetActive(i == index);
        }
        
    }
}
