using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class AccountSignUp : MonoBehaviour
{
    public List<SignUpCheckPanel> checkList;

    public Button emailSendBtn;

    public Button emailCheckBtn;

    public Toggle pwVibileToggle;
    
    public string settingNumber;

    public List<TMP_InputField> inputFieldlist;

    public int selectedInputIndex = -1;

    public GameObject visibleObject;
    public GameObject unvisibleObject;

    public void BackBtnClick()
    {
        UIManager.Instance.PopupListPop();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (selectedInputIndex < 0 || selectedInputIndex >= inputFieldlist.Count - 1) return;
            
            selectedInputIndex++;
            inputFieldlist[selectedInputIndex].Select();
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKey(KeyCode.Tab))
        {
            if (selectedInputIndex <= 0) return;
            
            selectedInputIndex--;
            inputFieldlist[selectedInputIndex].Select();
        }
    }

    public void TogglePasswordVisibility()
    {
        if (pwVibileToggle.isOn)
        {
            // 토글이 켜져 있으면 비밀번호를 표시
            inputFieldlist[3].contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            // 토글이 꺼져 있으면 비밀번호를 가림
            inputFieldlist[3].contentType = TMP_InputField.ContentType.Password;
        }
        
        visibleObject.SetActive(pwVibileToggle.isOn);
        unvisibleObject.SetActive(!pwVibileToggle.isOn);

        // 비밀번호 입력 필드를 업데이트하여 변경된 설정을 적용
        inputFieldlist[3].ForceLabelUpdate();
    }
    
    string GenerateAuthenticationCode()
    {
        System.Random random = new System.Random();
        int codeLength = 8;

        // StringBuilder를 사용하여 효율적으로 문자열 생성
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(codeLength);

        for (int i = 0; i < codeLength; i++)
        {
            int randomNumber = random.Next(0, 10); // 0부터 9 사이의 랜덤 숫자 생성
            stringBuilder.Append(randomNumber.ToString());
        }
        return stringBuilder.ToString();
    }

    public void InputSelected(int value)
    {
        selectedInputIndex = value;
    }
    
    public void CreateAccount()
    {
        CheckAllStatus();
    }
    
    public void CheckAllStatus()
    {
        int checkCount = 0;
        
        for (int i = 0; i < checkList.Count; i++)
        {
            if (checkList[i].isChecked == false)
            {
                checkCount++;
            }
        }

        if (checkCount > 0)
        {
            UIManager.Instance.OpenRecyclePopup("안내", "확인하지 않은 항목이 있습니다", null);
        }
        else
        {
            Debug.Log("계정 생성 시도");
            BackendManager.Instance.TryCustomSignin(checkList[2].input.text, checkList[3].input.text, checkList[0].input.text);
        }
    }

    public void CheckIDUsable()
    {
        SignUpCheckPanel target = checkList[2];
        
        if (target.input.text.Length > 20)
        {
            target.infoText.text = "<color=red>아이디가 너무 깁니다. 다시 시도해주세요.";
            target.isChecked = false;
        }
        else if (target.input.text.Length < 5)
        {
            target.infoText.text = "<color=red>아이디가 너무 짧습니다. 다시 시도해주세요.";
            target.isChecked = false;
        }
        else
        {
            target.infoText.text = "";
            target.isChecked = true;
        }
    }

    public void CheckPWUsable()
    {
        SignUpCheckPanel target = checkList[3];
        
        if (target.input.text.Length > 20)
        {
            target.infoText.text = "<color=red>비밀번호가 너무 깁니다. 다시 시도해주세요.";
            target.isChecked = false;
        }
        else if (target.input.text.Length < 5)
        {
            target.infoText.text = "<color=red>비밀번호가 너무 짧습니다. 다시 시도해주세요.";
            target.isChecked = false;
        }
        else if (!IsPasswordValid(target.input.text))
        {
            target.infoText.text = "<color=red>특수문자, 숫자, 그리고 적어도 하나의 영문자를 각각 하나 이상 포함해야 합니다.";
            target.isChecked = false;
        }
        else
        {
            target.infoText.text = "<color=black>사용 가능 합니다.";
            target.isChecked = true;
        }
    }

    // public void ReCheckPWUsable()
    // {
    //     SignUpCheckPanel target = checkList[2];
    //
    //     if (checkList[1].input.text != target.input.text)
    //     {
    //         Debug.Log("일치안함");
    //         target.infoText.text = "<color=red>비밀번호가 일치하지 않습니다.";
    //         target.isChecked = false;
    //     }
    //     else
    //     {
    //         Debug.Log("일치함");
    //         target.infoText.text = "<color=green>사용 가능 합니다.";
    //         target.isChecked = true;
    //     }
    // }

    public void EmailCheck()
    {
        SignUpCheckPanel target = checkList[1];

        if (checkList[1].input.text != settingNumber)
        {
            target.infoText.text = "<color=red>인증번호가 일치하지 않습니다. 다시 시도해주세요.";
            target.isChecked = false;
            
            emailSendBtn.interactable = true;
        }
        else
        {
            target.infoText.text = "<color=black>인증번호가 일치합니다.";
            checkList[0].isChecked = true;
            target.isChecked = true;
            emailCheckBtn.interactable = false;
        }
    }

    public void SendEmailTest()
    {
        SendEmailTestAsync();
    }
    
    public async Task SendEmailTestAsync()
    {
        SignUpCheckPanel target = checkList[0];

        MailMessage mail = new MailMessage();

        mail.From = new MailAddress("GangToeSal@gmail.com"); // 보내는사람

        if (target.input.text.Contains("@"))
        {
            mail.To.Add(target.input.text);
            target.infoText.text = "<color=black>인증번호가 전송 됐습니다.";

            settingNumber = GenerateAuthenticationCode();
            emailSendBtn.interactable = false;
        }
        else
        {
            target.infoText.text = "<color=red>이메일 형식이 올바르지 않습니다.";
            return;
        }

        mail.Subject = "강한토끼만 살아 남는다 인증 메일";

        mail.Body = $"이메일 인증 코드를 입력하세요\n \n본인 인증을 위해 자동으로 발송된 메일입니다.\n아래 인증번호를 사용하여 이메일 주소를 인증해주세요.\n{settingNumber}";

        using (SmtpClient smtpServer = new SmtpClient("smtp.gmail.com"))
        {
            smtpServer.Port = 587;

            smtpServer.Credentials = new System.Net.NetworkCredential("gangtoesal@gmail.com", "rmgnahrysuztsyof") as ICredentialsByHost; // 보내는사람 주소 및 비밀번호 확인

            smtpServer.EnableSsl = true;

            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

            await smtpServer.SendMailAsync(mail);
        }

        Debug.Log("success");
    }


    static bool IsPasswordValid(string password)
    {
        // 특수문자, 숫자, 그리고 적어도 하나의 영문자를 각각 하나 이상 포함해야 합니다.
        bool hasSpecialChar = password.Any(c => char.IsSymbol(c) || char.IsPunctuation(c));
        bool hasDigit       = password.Any(char.IsDigit);
        bool hasLetter      = password.Any(char.IsLetter);

        // 모든 조건을 만족하면 true를 반환합니다.
        return hasSpecialChar && hasDigit && hasLetter;
    }
}
