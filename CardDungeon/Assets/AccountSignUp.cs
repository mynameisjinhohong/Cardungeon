using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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

    public Button CreateAccountBtn;

    public string HiddenPassword;
    public string HiddenPasswordCheck;
    
    public string settingNumber;

    public void BackBtnClick()
    {
        UIManager.Instance.PopupListPop();
    }
    
    string GenerateAuthenticationCode()
    {
        System.Random random = new System.Random();
        int codeLength = 8;

        // StringBuilder�� ����Ͽ� ȿ�������� ���ڿ� ����
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(codeLength);

        for (int i = 0; i < codeLength; i++)
        {
            int randomNumber = random.Next(0, 10); // 0���� 9 ������ ���� ���� ����
            stringBuilder.Append(randomNumber.ToString());
        }
        return stringBuilder.ToString();
    }

    public void CreateAccount()
    {
        BackendManager.Instance.TryCustomSignin(checkList[0].input.text, checkList[1].input.text);
    }
    
    public bool CheckAllStatus()
    {
        int checkCount = 0;
        
        for (int i = 0; i < checkList.Count; i++)
        {
            if (checkList[i].isChecked == false)
            {
                checkCount++;
            }
        }
        
        CreateAccountBtn.interactable = checkCount > 0;

        return checkCount <= 0;
    }

    public void CheckIDUsable()
    {
        SignUpCheckPanel target = checkList[0];
        
        if (target.input.text.Length > 20)
        {
            target.infoText.text = "<color=red>���̵� �ʹ� ��ϴ�. �ٽ� �õ����ּ���.";
            target.isChecked = false;
        }
        else if (target.input.text.Length < 5)
        {
            target.infoText.text = "<color=red>���̵� �ʹ� ª���ϴ�. �ٽ� �õ����ּ���.";
            target.isChecked = false;
        }
        else
        {
            target.infoText.text = "<color=green>��� ������ ���̵��Դϴ�.";
            target.isChecked = true;
        }
    }

    public void CheckPWUsable()
    {
        SignUpCheckPanel target = checkList[1];
        
        if (target.input.text.Length > 20)
        {
            target.infoText.text = "<color=red>��й�ȣ�� �ʹ� ��ϴ�. �ٽ� �õ����ּ���.";
            target.isChecked = false;
        }
        else if (target.input.text.Length < 5)
        {
            target.infoText.text = "<color=red>��й�ȣ�� �ʹ� ª���ϴ�. �ٽ� �õ����ּ���.";
            target.isChecked = false;
        }
        else if (!IsPasswordValid(target.input.text))
        {
            target.infoText.text = "<color=red>Ư������, ����, �׸��� ��� �ϳ��� �����ڸ� ���� �ϳ� �̻� �����ؾ� �մϴ�.";
            target.isChecked = false;
        }
        else
        {
            target.infoText.text = "<color=green>��� ���� �մϴ�.";
            target.isChecked = true;
        }
    }

    public void ReCheckPWUsable()
    {
        SignUpCheckPanel target = checkList[2];

        if (checkList[1].input.text != target.input.text)
        {
            Debug.Log("��ġ����");
            target.infoText.text = "<color=red>��й�ȣ�� ��ġ���� �ʽ��ϴ�.";
            target.isChecked = false;
        }
        else
        {
            Debug.Log("��ġ��");
            target.infoText.text = "<color=green>��� ���� �մϴ�.";
            target.isChecked = true;
        }
    }

    public void EmailCheck()
    {
        SignUpCheckPanel target = checkList[4];

        if (checkList[4].input.text != settingNumber)
        {
            target.infoText.text = "<color=red>������ȣ�� ��ġ���� �ʽ��ϴ�. �ٽ� �õ����ּ���.";
            target.isChecked = false;
            
            emailSendBtn.interactable = true;
        }
        else
        {
            target.infoText.text = "<color=green>������ȣ�� ��ġ�մϴ�.";
            target.isChecked = true;

            CheckAllStatus();
            emailCheckBtn.interactable = false;
        }
    }
    
    public void SendEmailTest()
    {
        SignUpCheckPanel target = checkList[3];
        
        MailMessage mail = new MailMessage();
        
        mail.From = new MailAddress("GangToeSal@gmail.com"); // �����»��

        if (target.input.text.Contains("@"))
        {
            mail.To.Add(target.input.text);
            target.infoText.text = "<color=green>������ȣ�� ���� �ƽ��ϴ�.";
            
            settingNumber = GenerateAuthenticationCode();
            emailSendBtn.interactable = false;
        }
        else
        {
            target.infoText.text = "<color=red>�̸��� ������ �ùٸ��� �ʽ��ϴ�.";
            return;
        }

        mail.Subject = "�����䳢�� ��� ���´� ���� ����";

        mail.Body = $"�̸��� ���� �ڵ带 �Է��ϼ���\n \n���� ������ ���� �ڵ����� �߼۵� �����Դϴ�.\n�Ʒ� ������ȣ�� ����Ͽ� �̸��� �ּҸ� �������ּ���.\n{settingNumber}";
        
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

        smtpServer.Port = 587;

        smtpServer.Credentials = new System.Net.NetworkCredential("tkddnr9546@gmail.com", "cuqpixaxzxkvgdsw") as ICredentialsByHost; // �����»�� �ּ� �� ��й�ȣ Ȯ��

        smtpServer.EnableSsl = true;

        ServicePointManager.ServerCertificateValidationCallback =

            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)

            { return true; };

        smtpServer.Send(mail);

        Debug.Log("success");
    }

    static bool IsPasswordValid(string password)
    {
        // Ư������, ����, �׸��� ��� �ϳ��� �����ڸ� ���� �ϳ� �̻� �����ؾ� �մϴ�.
        bool hasSpecialChar = password.Any(c => char.IsSymbol(c) || char.IsPunctuation(c));
        bool hasDigit       = password.Any(char.IsDigit);
        bool hasLetter      = password.Any(char.IsLetter);

        // ��� ������ �����ϸ� true�� ��ȯ�մϴ�.
        return hasSpecialChar && hasDigit && hasLetter;
    }

    public void TogglePasswordVisibility(Toggle toggleTarget, InputField inputTarget)
    {
        if (toggleTarget.isOn)
        {
            // ����� ���� ������ ��й�ȣ�� ǥ��
            inputTarget.contentType = InputField.ContentType.Standard;
        }
        else
        {
            // ����� ���� ������ ��й�ȣ�� ����
            inputTarget.contentType = InputField.ContentType.Password;
        }

        // ��й�ȣ �Է� �ʵ带 ������Ʈ�Ͽ� ����� ������ ����
        inputTarget.ForceLabelUpdate();
    }
}
