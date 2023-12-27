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
                Debug.Log("�α��ο� �����߽��ϴ�");
            }
            else
            {
                string errMSG = "";
                
                switch (callback.GetStatusCode())
                {
                    case "401" :
                        errMSG = "������ �������� �ʰų�\n���̵� ��й�ȣ�� Ʋ�Ƚ��ϴ�.";
                        break;
                    case "403" :
                        errMSG = "���ܵ� �����Դϴ�, �� ���ͷ� ���� ���ּ���.";
                        break;
                    case "400" :
                        errMSG = "�߸��� ������� �Դϴ�.";
                        break;
                    case "410" :
                        errMSG = "������ ���� ���� �Դϴ�.";
                        break;
                }
                
                Debug.Log("�����ڵ� : " + callback.GetErrorCode() + "���� ����" + callback.GetMessage());
                
                UIManager.Instance.OpenRecyclePopup("�α��� ����", errMSG, null);
            }
        });
    }

    public void FindAccountClick()
    {
        GameObject findAccount = UIManager.Instance.FindAccountPrefab;
        
        UIManager.Instance.OpenPopup(findAccount);
    }
}
