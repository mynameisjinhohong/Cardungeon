using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class AccountSignUp : MonoBehaviour
{
    public List<SignUpCheckPanel> checkList;
    
    public void BackBtnClick()
    {
        UIManager.Instance.PopupListPop();
    }

    public void SendEmailTest()
    {
        MailMessage mail = new MailMessage();
        
        mail.From = new MailAddress("uuuuufgh@gmail.com"); // �����»��

        mail.To.Add("tkddnr9546@gmail.com"); // �޴� ���

        mail.Subject = "���� ���� �׽�Ʈ";

        mail.Body = "This is for testing SMTP mail from GMAIL";
        
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

        smtpServer.Port = 587;

        smtpServer.Credentials = new System.Net.NetworkCredential("uuuuufgh@gmail.com", "tkddnrWkdWkd123") as ICredentialsByHost; // �����»�� �ּ� �� ��й�ȣ Ȯ��

        smtpServer.EnableSsl = true;

        ServicePointManager.ServerCertificateValidationCallback =

            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)

            { return true; };

        smtpServer.Send(mail);

        Debug.Log("success");
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
}
