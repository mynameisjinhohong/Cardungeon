using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.SimpleGoogleSignIn.Scripts;
using BackEnd;
using TMPro;

namespace Assets.SimpleGoogleSignIn
{
    public class Example : Singleton<Example>
    {
        public GoogleAuth GoogleAuth;
        private String Log;
        public String Output;

        [SerializeField]
        private UserInfo googleUserData;
        
        public void Start()
        {
            Application.logMessageReceived += (condition, _, _) => Log += condition + '\n';
            GoogleAuth = new GoogleAuth();
        }

        public void SignIn()
        {
            GoogleAuth.SignIn(OnSignIn, caching: true);
        }

        public void SignOut()
        {
            GoogleAuth.SignOut(revokeAccessToken: true);
            Output = "Not signed in";
        }

        public void GetAccessToken()
        {
            GoogleAuth.GetAccessToken(OnGetAccessToken);
        }

        private void OnSignIn(bool success, string error, UserInfo userInfo)
        {
            Output = success ? $"Hello, {userInfo.name}!" : error;

            googleUserData = userInfo;

            GetAccessToken();
            
            BackendManager.Instance.userInfo.UserIndate = Backend.UserInDate;
            BackendManager.Instance.userInfo.Nickname = Backend.UserNickName;
            BackendManager.Instance.userInfo.UID = Backend.UID;
        }

        private void OnGetAccessToken(bool success, string error, TokenResponse tokenResponse)
        {
            Output = success ? $"Access token: {tokenResponse.AccessToken}" : error;

            if (!success) return;

            var jwt = new JWT(tokenResponse.IdToken);

            Debug.Log($"JSON Web Token (JWT) Payload: {jwt.Payload}");
            
            jwt.ValidateSignature(GoogleAuth.ClientId, OnValidateSignature);
            
            BackendManager.Instance.TryAuthorizeFederation(tokenResponse.IdToken);
            
            Debug.Log(Log);
        }

        private void OnValidateSignature(bool success, string error)
        {
            Output += Environment.NewLine;
            Output += success ? "JWT signature validated" : error;
        }

        public void Navigate(string url)
        {
            Application.OpenURL(url);
        }
    }
}