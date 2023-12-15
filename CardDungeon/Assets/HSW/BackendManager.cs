using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using BackEnd;
using BackEnd.Util;
using LitJson;
using Newtonsoft.Json;
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
    public int initTimeCount = 0;
    
    public int SuccessLoadDataCount = 0;

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
        StartCoroutine((LoginProcess(bro, LoginType.Guest)));
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
    
    private IEnumerator LoginProcess(BackendReturnObject bro, LoginType type)
    {
        if (!bro.IsSuccess())
        {
            PlayerPrefs.SetInt("LoginWay", -1);
            Debug.LogError($"{bro.IsSuccess()} / {bro.GetStatusCode()} / {bro.GetErrorCode()} / {bro.GetMessage()}");
            switch (bro.GetStatusCode())
            {
                case "401": //서버점검
                    switch (bro.GetErrorCode())
                    {
                        case "BadUnauthorizedException":
                            if (bro.GetMessage().Contains("accessToken") || bro.GetMessage().Contains("refreshToken"))
                            {
                                Debug.LogError("accessToken or refreshToken 만료");
                            }
                            else if (bro.GetMessage().Contains("maintenance")){
                                BackendReturnObject Bro = Backend.Utils.GetServerStatus();
                                if (int.Parse(Bro.GetReturnValuetoJSON()["serverStatus"].ToString()) ==
                                    (int)ServerState.Maintenance)
                                {
                                    // 나중에 점검중 추가할것
                                }
                            }else if (bro.GetMessage().Contains("customId"))
                            {
                                Debug.LogError($"Guest Data Damaged");
                            }
                            break;
                    }
                    break;
                case "403": //에러
                    if(bro.GetMessage().Contains("blocked "))
                    {
                        Debug.LogError($"{bro.GetErrorCode()}");
                    }
                    break;
            }
            yield break;
        }
        switch (type)
        {
            case LoginType.Guest:
                GetUserInfo();
                Debug.LogError("게스트 로그인에 성공했습니다.");
                break;
            case LoginType.Auto:
                GetUserInfo();
                Debug.LogError("자동 로그인에 성공했습니다.");
                break;
        }
        UserIndate = Backend.UserInDate;
        GetServerTime();
        // while (!LoadServerTime)
        //     yield return null;
        // Debug.LogError($"LoadServerTime {LoadServerTime}");
        // GetServerChartList();
        // while (!LoadChartList)
        //     yield return null;
        // Debug.LogError($"LoadChartList {LoadChartList}");
        // GetChartData();
        // while (!LoadChartDataFromServer)
        //     yield return null;
        // Debug.LogError($"LoadChartDataFromServer {LoadChartDataFromServer}");
        // LoadLocalData();
        // while(!LoadChartDataFromLocal)
        //     yield return null;
        // Debug.LogError($"LoadChartDataFromLocal {LoadChartDataFromLocal}");
        // DataManager.Instance.SetDefaultData();
        if (type == LoginType.Auto)
        {
            switch (bro.GetStatusCode())
            {
                case "201": //로그인
                    Debug.Log("자동 로그인 하여 서버에서 유저 데이터 불러오기 성공");
                    break;
            }
        }
        else
        {
            switch (bro.GetStatusCode())
            {
                case "201": //신규 회원가입
                    Debug.Log("신규 회원으로 시작합니다");
                    InsertLog(GameLogType.Signin, $"{type}/{Application.version}");
                    break;
                case "200": //로그인
                    Debug.Log("일반 로그인");
                    InsertLog(GameLogType.Login, $"{type}/{Application.version}");
                    break;
            }
        }
        StartCoroutine(nameof(RefreshToken));
    }
        private IEnumerator RefreshToken()
    {
        int count = 0;
        WaitForSeconds waitForRefreshTokenCycle = new WaitForSeconds(60 * 60 * 6); // 60초 x 60분 x 8시간
        WaitForSeconds waitForRetryCycle = new WaitForSeconds(15f);
        // 첫 호출시에는 리프레시 토큰하지 않도록 8시간을 기다리게 해준다.
        bool isStart = false;
        if (!isStart)
        {
            isStart = true;
            yield return waitForRefreshTokenCycle; // 8시간 기다린 후 반복문 시작
        }
        BackendReturnObject bro = null;
        bool isRefreshSuccess = false;
        // 이후부터는 반복문을 돌면서 8시간마다 최대 3번의 리프레시 토큰을 수행하게 된다.
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                bro = Backend.BMember.RefreshTheBackendToken();
                Debug.Log("리프레시 토큰 성공 여부 : " + bro);
                if (bro.IsSuccess())
                {
                    Debug.Log("토큰 재발급 완료");
                    isRefreshSuccess = true;
                    break;
                }
                else
                {
                    if (bro.GetMessage().Contains("bad refreshToken"))
                    {
                        Debug.LogError("중복 로그인 발생");
                        isRefreshSuccess = false;
                        break;
                    }
                    else
                    {
                        Debug.LogWarning("15초 뒤에 토큰 재발급 다시 시도");
                    }
                }
                yield return waitForRetryCycle; // 15초 뒤에 다시시도
            }
            // 3번 이상 재시도시 에러가 발생할 경우, 리프레시 토큰 에러 외에도 네트워크 불안정등의 이유로 이후에도 로그인에 실패할 가능성이 높습니다. 
            // 유저들이 스스로 토큰 리프레시를 할수 있도록 구현해주시거나 수동 로그인을 하도록 구현해주시기 바랍니다.
            if (bro == null)
            {
                Debug.LogError("토큰 재발급에 문제가 발생했습니다. 수동 로그인을 시도해주세요");
                //팝업 띄울것
                StopCoroutine(nameof(RefreshToken));
            }
            if (!bro.IsSuccess())
            {
                Debug.LogError("토큰 재발급에 문제가 발생하였습니다 : " + bro);
                //팝업 띄울것
                StopCoroutine(nameof(RefreshToken));
            }
            // 성공할 경우 값 초기화 후 8시간을 wait합니다.
            if (isRefreshSuccess)
            {
                Debug.Log("8시간 토큰 재 호출");
                isRefreshSuccess = false;
                count = 0;
                yield return waitForRefreshTokenCycle;
            }
        }
    }
    public void GetServerTime()
    {
        InitTime();
        InvokeRepeating("GetServerTimeFor5minutes", 300f, 300f);
    }
    
    public void InsertLog(GameLogType type, string str)
    {
        Param param = new();
        param.Add("Key", str);

        SendLog(type, param);
    }
    
    public void SendLog(GameLogType type, Param param)
    {
        string paramToString = JsonConvert.SerializeObject(param.GetValue());
        Debug.Log($"{type} / {paramToString}");
        Backend.GameLog.InsertLog($"{type}", param, 30, (callback) =>
        {
        });
    }
    public void InitTime()
    {
        if(initTimeCount > 3)
        {
            Debug.LogError(initTimeCount);
            //GameManager.Instance.UiManager.AlertManager.Initialize(AlertType.DataLoadError);
            return;
        }

        Backend.Utils.GetServerTime((callback) =>
        {
            if (callback.IsSuccess())
            {
                string time = callback.GetReturnValuetoJSON()["utcTime"].ToString();
                DateTime parsedDate = DateTime.Parse(time);
                //DataManager.Instance.SetLocalTime(parsedDate);
                LoadServerTime = true;
            }
            else
            {
                initTimeCount++;
                InitTime();
            }
        });
    }
    
    public void GetUserInfo()
    {
        UID = Backend.UID;
        Nickname = Backend.UserNickName;
    }
}