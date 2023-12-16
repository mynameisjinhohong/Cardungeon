using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using LitJson;
using System;
using UnityEngine.UI;

public class LoginTest : MonoBehaviour {
    
    [SerializeField] private InputField _inputField;
    void Start() {
        var bro = Backend.Initialize();
        Debug.Log("초기화 결과 : " + bro);
    }

    public void SignUp() {
        var bro = Backend.BMember.CustomSignUp(_inputField.text, _inputField.text);

        if (bro.IsSuccess()) {
            Debug.Log("회원가입 : " + bro);

        } else {
            Debug.LogError("회원가입 : " + bro);
        }
    }

    public void Login() {
        var bro = Backend.BMember.CustomLogin(_inputField.text, _inputField.text);

        if (bro.IsSuccess()) {
            Debug.Log("로그인 : " + bro);

        } else {
            Debug.LogError("로그인 : " + bro);
        }
    }

    public void UpdateNickname() {
        var bro = Backend.BMember.UpdateNickname(_inputField.text);

        if (bro.IsSuccess()) {
            gameObject.SetActive(false);
            Debug.Log("닉네임 변경 : " + bro);

        } else {
            Debug.LogError("닉네임 변경 : " + bro);
        }
    }

    void Update() {
        if (Backend.IsInitialized) {
            Backend.AsyncPoll();
        }
    }
}