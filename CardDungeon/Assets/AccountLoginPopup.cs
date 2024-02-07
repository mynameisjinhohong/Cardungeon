using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountLoginPopup : MonoBehaviour
{
    [SerializeField]
    TMP_InputField idInput;

    [SerializeField]
    TMP_InputField pwInput;

    [SerializeField]
    Transform idFindObject;

    [SerializeField]
    Transform pwFindObject;

    [SerializeField]
    Toggle pwVisibleToggle;

    [SerializeField] 
    float duration;

    [SerializeField]
    GameObject visibleImage;

    [SerializeField]
    GameObject invisibleImage;

    public bool isClickedFindAccount;

    private bool isMoving;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (idInput.isFocused)
            {
                pwInput.Select();
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Tab))
        {
            if (pwInput.isFocused)
            {
                idInput.Select();
            }
        }
    }

    public void BackBtnClick()
    {
        UIManager.Instance.PopupListPop();
    }

    public void AccountSignUpClick()
    {
        GameObject accountSignUp = UIManager.Instance.AccountSignUpPrefab;

        try
        {
            UIManager.Instance.OpenPopup(accountSignUp);
        }
        catch
        {
            Debug.Log("생성실패");
        }
        
    }

    public void AccountLoginCheck()
    {
        Backend.BMember.CustomLogin( idInput.text, pwInput.text, callback => {
            if(callback.IsSuccess())
            {
                Debug.Log("계정 로그인 성공");
                BackendManager.Instance.GetUserInfo();
                BackendManager.Instance.userInfo.playerID = idInput.text;
                MatchController.Instance.ChangeUI(1);
            }
            else
            {
                string errMSG = "";
                
                switch (callback.GetStatusCode())
                {
                    case "401" :
                        errMSG = "없는 계정이거나\n비밀번호가 틀렸습니다.";
                        break;
                    case "403" :
                        errMSG = "차단된 계정입니다. 고객센터로 문의하세요.";
                        break;
                    case "400" :
                        errMSG = "기기 정보가 없습니다.";
                        break;
                    case "410" :
                        errMSG = "탈퇴된 계정입니다.";
                        break;
                }

                UIManager.Instance.OpenRecyclePopup("로그인 실패", errMSG, null);
            }
        });
    }

    public void TogglePasswordVisibility()
    {
        if (pwVisibleToggle.isOn)
        {
            // 토글이 켜져 있으면 비밀번호를 표시
            pwInput.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            // 토글이 꺼져 있으면 비밀번호를 가림
            pwInput.contentType = TMP_InputField.ContentType.Password;
        }
        
        visibleImage.SetActive(pwVisibleToggle.isOn);
        invisibleImage.SetActive(!pwVisibleToggle.isOn);

        // 비밀번호 입력 필드를 업데이트하여 변경된 설정을 적용
        pwInput.ForceLabelUpdate();
    }

    public void FindAccountAnim()
    {
        if (isMoving) return;
        
        if (!isClickedFindAccount)
        {
            StartCoroutine(MoveObjectCoroutine(idFindObject, -55));
            StartCoroutine(MoveObjectCoroutine(pwFindObject, -110));
        }
        else
        {
            StartCoroutine(MoveObjectCoroutine(idFindObject, 0));
            StartCoroutine(MoveObjectCoroutine(pwFindObject, 0));
        }
        isClickedFindAccount =! isClickedFindAccount;
    }
    
    IEnumerator MoveObjectCoroutine(Transform objTransform, float targetY)
    {
        isMoving = true;
        
        if (!isClickedFindAccount)
        {
            idFindObject.gameObject.SetActive(true);
            pwFindObject.gameObject.SetActive(true);
        }
        
        float initialLocalY = objTransform.localPosition.y;
        float t = 0f;
        
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float newLocalY = Mathf.Lerp(initialLocalY, targetY, t);
            objTransform.localPosition = new Vector3(objTransform.localPosition.x, newLocalY, objTransform.localPosition.z);
            yield return null;
        }
        
        if(isClickedFindAccount)
            yield return new WaitForSeconds(duration - 0.3f);
        
        idFindObject.gameObject.SetActive(isClickedFindAccount);
        pwFindObject.gameObject.SetActive(isClickedFindAccount);

        yield return new WaitForSeconds(0.1f);
        isMoving = false;
    }
}
