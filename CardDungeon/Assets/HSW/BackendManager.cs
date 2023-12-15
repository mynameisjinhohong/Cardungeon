using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using BackEnd;
using BackEnd.Util;
using UnityEngine;

public class BackendManager : Singleton<BackendManager>
{
    private Thread serverCheckThread;
    
    public string UserIndate = string.Empty;
    public string Nickname   = string.Empty;
    public string UID        = string.Empty;
    
    public bool LoadChartList = false;
    public bool LoadChartDataFromServer = false;
    public bool LoadServerTime = false;
    public bool LoadChartDataFromLocal = false;

    public int checkLoginWayData = -1;
    public bool isInitialize = false;

    void Start()
    {
        Initialize();
    }
    
    public void Initialize()
    {
        BackendCustomSetting settings = new BackendCustomSetting();

        settings.clientAppID     = "5544b740-9b5b-11ee-9f65-b5daf02063d76483";
        settings.signatureKey    = "5544b741-9b5b-11ee-9f65-b5daf02063d76483";
        settings.functionAuthKey = "";
        settings.isAllPlatform   = true;
        settings.sendLogReport   = true;
        settings.timeOutSec      = 100;
        settings.useAsyncPoll    = true;

        var bro = Backend.Initialize(settings);

        if (bro.IsSuccess())
        {
            Debug.Log("Backend Initialize Success : " + bro);

            Backend.ErrorHandler.InitializePoll(true);

            Backend.ErrorHandler.OnMaintenanceError = () => {
                Debug.LogError("Server Inspection Error");
                //GetTempNotice();
            };
            Backend.ErrorHandler.OnTooManyRequestError = () => {
                Debug.LogError("403 Error");
            };
            Backend.ErrorHandler.OnTooManyRequestByLocalError = () => {
                Debug.LogError("403 Local Error");
            };
            Backend.ErrorHandler.OnOtherDeviceLoginDetectedError = () => {
                Debug.LogError("Cannot Refresh");
            };
            Backend.ErrorHandler.OnDeviceBlockError = () => {
                Debug.Log("Device Access Denied");
            };
            
            if (Application.platform == RuntimePlatform.Android)
            {
                //CheckLastVersion();
            }

            isInitialize = true;
        }
        else
        {
            Debug.Log("Initialize Failed : " + bro);
        }
        
    }
    
    public void GuestLoginSequense()
    {
        BackendReturnObject bro = Backend.BMember.GuestLogin("GuestLoginTry2 Sequence");
        Debug.LogError($"{bro.IsSuccess()} {bro.GetStatusCode()} {bro.GetErrorCode()} {bro.GetMessage()}");
        //StartCoroutine((LoginProcess(bro, LoginType.Guest)));
        PlayerPrefs.SetInt("LoginWay", 0);
    }

    private IEnumerator Polling()
    {
        while (true)
        {
            Backend.AsyncPoll();
            Backend.ErrorHandler.Poll();
            yield return null;
        }
    }
    
    // private IEnumerator LoginProcess(BackendReturnObject bro, LoginType type)
    // {
    //     if (!bro.IsSuccess())
    //     {
    //         PlayerPrefs.SetInt("LoginWay", -1);
    //         Debug.LogError($"{bro.IsSuccess()} / {bro.GetStatusCode()} / {bro.GetErrorCode()} / {bro.GetMessage()}");
    //         switch (bro.GetStatusCode())
    //         {
    //             case "401": //서버점검
    //                 switch (bro.GetErrorCode())
    //                 {
    //                     case "BadUnauthorizedException":
    //                         if (bro.GetMessage().Contains("accessToken") || bro.GetMessage().Contains("refreshToken"))
    //                         {
    //                             PopupManager.Instance.RecyclePopupCreate("1006","2033", true, () =>
    //                             {
    //                                 LoginUI.SetActive(true);
    //                             }, false);
    //                             Debug.LogError("accessToken or refreshToken 만료");
    //                         }
    //                         else if (bro.GetMessage().Contains("maintenance")){
    //                             BackendReturnObject Bro = Backend.Utils.GetServerStatus();
    //                             if (int.Parse(Bro.GetReturnValuetoJSON()["serverStatus"].ToString()) ==
    //                                 (int)ServerState.Maintenance)
    //                             {
    //                                 // 나중에 점검중 추가할것
    //                             }
    //                         }else if (bro.GetMessage().Contains("customId"))
    //                         {
    //                             PopupManager.Instance.RecyclePopupCreate("1006", "1006", true, () =>
    //                             {
    //                                 GuestIdDelete();
    //                                 LoginUI.SetActive(true);
    //                             }, false);
    //                             Debug.LogError($"Guest Data Damaged");
    //                         }
    //                         break;
    //                 }
    //                 break;
    //             case "403": //에러
    //                 if(bro.GetMessage().Contains("blocked "))
    //                 {
    //                     Debug.LogError($"{bro.GetErrorCode()}");
    //                     PopupManager.Instance.RecyclePopupCreate("1026", $"{2061}", true, (
    //                         Application.Quit), false);
    //                 }
    //                 break;
    //         }
    //         yield break;
    //     }
    //     switch (type)
    //     {
    //         case LoginType.Google:
    //             GetUserInfo();
    //             Debug.LogError("구글 로그인에 성공했습니다.");
    //             break;
    //         case LoginType.Apple:
    //             GetUserInfo();
    //             Debug.LogError("애플 로그인에 성공했습니다.");
    //             break;
    //         case LoginType.Guest:
    //             GetUserInfo();
    //             Debug.LogError("게스트 로그인에 성공했습니다.");
    //             break;
    //         case LoginType.Auto:
    //             GetUserInfo();
    //             Debug.LogError("자동 로그인에 성공했습니다.");
    //             break;
    //     }
    //     UserIndate = Backend.UserInDate;
    //     GetServerTime();
    //     while (!LoadServerTime)
    //         yield return null;
    //     Debug.LogError($"LoadServerTime {LoadServerTime}");
    //     GetServerChartList();
    //     while (!LoadChartList)
    //         yield return null;
    //     Debug.LogError($"LoadChartList {LoadChartList}");
    //     GetChartData();
    //     while (!LoadChartDataFromServer)
    //         yield return null;
    //     Debug.LogError($"LoadChartDataFromServer {LoadChartDataFromServer}");
    //     LoadLocalData();
    //     while(!LoadChartDataFromLocal)
    //         yield return null;
    //     Debug.LogError($"LoadChartDataFromLocal {LoadChartDataFromLocal}");
    //     DataManager.Instance.SetDefaultData();
    //     if (type == LoginType.Auto)
    //     {
    //         switch (bro.GetStatusCode())
    //         {
    //             case "201": //로그인
    //                 Debug.Log("자동 로그인 하여 서버에서 유저 데이터 불러오기 성공");
    //                 GetUserDataFromServer();
    //                 InsertLog(GameLogType.Login, $"{type}/{Application.version}");
    //                 break;
    //         }
    //     }
    //     else
    //     {
    //         switch (bro.GetStatusCode())
    //         {
    //             case "201": //신규 회원가입
    //                 Debug.Log("신규 회원으로 시작합니다");
    //                 SetNewUserDataSaveToServer();
    //                 //SetCountryCode();
    //                 InsertLog(GameLogType.Signin, $"{type}/{Application.version}");
    //                 break;
    //             case "200": //로그인
    //                 Debug.Log("일반 로그인");
    //                 GetUserDataFromServer();
    //                 InsertLog(GameLogType.Login, $"{type}/{Application.version}");
    //                 break;
    //         }
    //     }
    //     yield return new WaitUntil(() => SuccessLoadDataCount == typeof(UserDataType).GetEnumNames().Length);
    //     DataManager.Instance.LoadStatus = true;
    //     DataManager.Instance.DataLoadComplete();
    //     StartCoroutine(nameof(RefreshToken));
    // }
}
