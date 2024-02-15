using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.SimpleGoogleSignIn.Scripts;
using BackEnd;
using TMPro;

namespace Assets.SimpleGoogleSignIn
{
    public class Example : MonoBehaviour
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

            BackendManager.Instance.GetServerTime();
            
            DataManager.Instance.userData.rowIndate = Backend.UserInDate;

            if (GoogleAuth.SavedAuth == null)
            {
                DataManager.Instance.SaveUserBattleInfo(ServerSaveType.Insert);

                BackendManager.Instance.SendTransaction(TransactionType.Insert);
            }
            else
            {
                DataManager.Instance.SaveUserBattleInfo(ServerSaveType.Update);

                BackendManager.Instance.SendTransaction(TransactionType.Update);
            }
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
            
            BackendManager.Instance.GetUserInfo();
            //BackendManager.Instance.userInfo.playerID = userInfo.email;
            //BackendManager.Instance.userInfo.Email = userInfo.email;
            MatchController.Instance.ChangeUI(1);
        }

        private void OnGetAccessToken(bool success, string error, TokenResponse tokenResponse)
        {
            Output = success ? $"Access token: {tokenResponse.AccessToken}" : error;

            if (!success) return;

            var jwt = new JWT(tokenResponse.IdToken);

            Debug.Log($"JSON Web Token (JWT) Payload: {jwt.Payload}");
            
            jwt.ValidateSignature(GoogleAuth.ClientId, OnValidateSignature);
            
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