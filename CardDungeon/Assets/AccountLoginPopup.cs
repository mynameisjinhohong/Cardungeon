using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountLoginPopup : MonoBehaviour
{
    [SerializeField]
    InputField idInput;

    [SerializeField]
    InputField pwInput;
    
    public void BackBtnClick()
    {
        UIManager.Instance.PopupListPop();
    }

    public void AccountSignUpClick()
    {
        GameObject accountSignUp = UIManager.Instance.AccountSignUpPrefab;
        
        UIManager.Instance.OpenPopup(accountSignUp);
    }

    public void AccountLoginCheck()
    {
        Backend.BMember.CustomLogin( idInput.text, pwInput.text, callback => {
            if(callback.IsSuccess())
            {
                Debug.Log("로그인에 성공했습니다");
            }
            else
            {
                string errMSG = "";
                
                switch (callback.GetStatusCode())
                {
                    case "401" :
                        errMSG = "계정이 존재하지 않거나\n아이디나 비밀번호가 틀렸습니다.";
                        break;
                    case "403" :
                        errMSG = "차단된 유저입니다, 고객 센터로 문의 해주세요.";
                        break;
                    case "400" :
                        errMSG = "잘못된 기기정보 입니다.";
                        break;
                    case "410" :
                        errMSG = "삭제된 유저 정보 입니다.";
                        break;
                }
                
                Debug.Log("에러코드 : " + callback.GetErrorCode() + "에러 사유" + callback.GetMessage());
                
                UIManager.Instance.OpenRecyclePopup("로그인 실패", errMSG, null);
            }
        });
    }

    public void FindAccountClick()
    {
        GameObject findAccount = UIManager.Instance.FindAccountPrefab;
        
        UIManager.Instance.OpenPopup(findAccount);
    }
}
