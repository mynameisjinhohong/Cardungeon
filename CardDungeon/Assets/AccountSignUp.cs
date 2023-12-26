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
        
        mail.From = new MailAddress("uuuuufgh@gmail.com"); // 보내는사람

        mail.To.Add("tkddnr9546@gmail.com"); // 받는 사람

        mail.Subject = "메일 전송 테스트";

        mail.Body = "This is for testing SMTP mail from GMAIL";
        
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

        smtpServer.Port = 587;

        smtpServer.Credentials = new System.Net.NetworkCredential("uuuuufgh@gmail.com", "tkddnrWkdWkd123") as ICredentialsByHost; // 보내는사람 주소 및 비밀번호 확인

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

        // StringBuilder를 사용하여 효율적으로 문자열 생성
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(codeLength);

        for (int i = 0; i < codeLength; i++)
        {
            int randomNumber = random.Next(0, 10); // 0부터 9 사이의 랜덤 숫자 생성
            stringBuilder.Append(randomNumber.ToString());
        }

        return stringBuilder.ToString();
    }
}
