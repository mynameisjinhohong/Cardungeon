using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Assets.SimpleGoogleSignIn;
using Assets.SimpleGoogleSignIn.Scripts;
using BackEnd;
using BackEnd.Tcp;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackendManager : Singleton<BackendManager>
{
    private static BackendManager instance;   // 인스턴스
    
    private Thread serverCheckThread;

    public ServerType serverType;
    public PlatformType platformType;

    public int SuccessLoadDataCount = 0;
    
    [Header("유저정보")]
    public UserInfo userInfo;
    
    public bool LoadServerTime = false;
    public int checkLoginWayData = -1;
    public bool isInitialize = false;
    public bool isLoadGame   = false;
    public bool UseAutoLogin = false; 
    public int matchIndex = 0;
    public bool isMatching = false;
    public bool isMeSuperGamer = false;

    [Header("전체 유저 데이터 리스트")] 
    public List<UserData> UserDataList;
    
    //[Header("방정보")]
    //public RoomSettingData roomSettingData;

    public bool isFastMatch;

    public string inviterName;
    
    private int initTimeCount = 0;

    [SerializeField]
    List<TransactionValue> transactionList = new List<TransactionValue>();
    
    void Start()
    {
        if(!isInitialize)
            Initialize();
    }
    
    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
        // 모든 씬에서 유지
        DontDestroyOnLoad(this.gameObject);
        //SetResolution();

        if (!PlayerPrefs.HasKey("UseAutoLogin"))
            UseAutoLogin = false;
        else
            UseAutoLogin = CheckAutoLoginUse();
    }

    public bool CheckAutoLoginUse()
    {
        Debug.Log("오토로그인 검증" + PlayerPrefs.GetInt("UseAutoLogin"));
        
        return PlayerPrefs.GetInt("UseAutoLogin") == 1;
    }
    public void SetResolution()
    {
        int setWidth = 1920; // 사용자 설정 너비
        int setHeight = 1080; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }
    public void Initialize()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android :
                platformType = PlatformType.Android;
                break;
            case RuntimePlatform.WindowsEditor :
                platformType = PlatformType.Window;
                break;
            case RuntimePlatform.IPhonePlayer :
                platformType = PlatformType.IOS;
                break;
        }

        BackendCustomSetting settings = new BackendCustomSetting();

        if (serverType == ServerType.Dev)
        {
            settings.clientAppID     = "5544b740-9b5b-11ee-9f65-b5daf02063d76483";
            settings.signatureKey    = "5544b741-9b5b-11ee-9f65-b5daf02063d76483";
            settings.functionAuthKey = "";
            settings.isAllPlatform   = true;
            settings.sendLogReport   = true;
            settings.timeOutSec      = 100;
            settings.useAsyncPoll    = true;
        }
        else
        {
            settings.clientAppID     = "c06c8520-a268-11ee-9006-8513c03d25496583";
            settings.signatureKey    = "c06c8521-a268-11ee-9006-8513c03d25496583";
            settings.functionAuthKey = "";
            settings.isAllPlatform   = true;
            settings.sendLogReport   = true;
            settings.timeOutSec      = 100;
            settings.useAsyncPoll    = true;
        }
        
        var bro = Backend.Initialize(settings);

        if (bro.IsSuccess())
        {
            Debug.Log("Backend Initialize Success : " + bro);

            Backend.ErrorHandler.InitializePoll(true);

            Backend.ErrorHandler.OnMaintenanceError = () => {
                Debug.LogError("Server Inspection Error");
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
            
            StartCoroutine(nameof(Polling));

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //CheckLastVersion();
            }
            
            CheckLoginWayData();

            isInitialize = true;
        }
        else
        {
            Debug.Log("Initialize Failed : " + bro);
        }
        
    }
    
    public void CheckLoginWayData()
    {
        if (PlayerPrefs.HasKey("LoginWay"))
        {
            checkLoginWayData = PlayerPrefs.GetInt("LoginWay");
        }
        Debug.LogError(PlayerPrefs.HasKey("LoginWay") + checkLoginWayData.ToString());
    }

    // public void GuestLoginSequense()
    // {
    //     BackendReturnObject bro = Backend.BMember.GuestLogin("GuestLoginTry2 Sequence");
    //     Debug.LogError($"{bro.IsSuccess()} {bro.GetStatusCode()} {bro.GetErrorCode()} {bro.GetMessage()}");
    //
    //     if (!bro.IsSuccess())
    //     {
    //         Debug.Log("계스트 계정 생성 실패");
    //         GuestIdDelete();
    //     }
    //
    //     StartCoroutine((LoginProcess(bro, LoginType.Guest)));
    //     PlayerPrefs.SetInt("LoginWay", 0);
    // }
    private IEnumerator Polling()
    {
        while (true)
        {
            Backend.AsyncPoll();
            Backend.ErrorHandler.Poll();
            yield return null;
        }
    }

    public void TryCustomSignin(string id, string pw, string email)
    {
        GameObject nicknamePopup = UIManager.Instance.NickNamePrefab;
        
        Backend.BMember.CustomSignUp ( id, pw, callback => {
            if(callback.IsSuccess())
            {
                Debug.Log("회원가입에 성공했습니다.");
                
                Backend.BMember.UpdateCustomEmail(email, ( callback ) =>
                {
                    UIManager.Instance.PopupListPop();
                
                    UIManager.Instance.PopupListPop();
                
                    Debug.Log("신규 회원으로 시작합니다");
                    SetNewUserDataSaveToServer();

                    // 닉네임 생성 팝업 만들기
                    UIManager.Instance.OpenPopup(nicknamePopup);

                    PlayerPrefs.SetInt("LoginWay", 0);

                    checkLoginWayData = PlayerPrefs.GetInt("LoginWay");
                });
            }
            else
            {
                Debug.LogWarning(callback.GetStatusCode() + callback.GetMessage());

                string errmesage = "";
                
                switch (callback.GetStatusCode())
                {
                    case "409" :
                        errmesage = "중복된 아이디 입니다.";
                        break;
                    case "401" :
                        errmesage = "서버상태가 좋지 않습니다.\n다시 시도해주세요.";
                        break;
                    case "400" :
                        errmesage = "아이디와 이메일을 다시 확인 해주세요.";
                        break;
                    case "403" :
                        errmesage = "차단당한 계정입니다.\n고객센터로 문의 주세요";
                        break;
                }
                UIManager.Instance.OpenRecyclePopup("안내", $"{errmesage}", null);
            }
        });
    }
    
    public void StartTokenLogin()
    {
        Debug.Log("자동 로그인");

        switch (checkLoginWayData)
        {
            case 0 :
                Debug.Log("0으로 로그인");
                Backend.BMember.LoginWithTheBackendToken((callback) =>
                {
                    StartCoroutine(LoginProcess(callback, LoginType.Auto));
                });
                break;
            case 1 :
                Debug.Log("1으로 로그인");
                Example.Instance.SignIn();
                break;
        }
        
    }

    public void TryAuthorizeFederation(string Token)
    {
        Debug.Log("구글 토큰 로그인 시도" + Token);
        Backend.BMember.AuthorizeFederation ( Token, FederationType.Google, "Google로 가입함", callback =>
        {
            userInfo.UserIndate = Backend.UserInDate;
            userInfo.Nickname = Backend.UserNickName;
            userInfo.UID = Backend.UID;
            
            PlayerPrefs.SetInt("LoginWay", 1); 
            CheckLoginWayData();
            
            CheckNickNameCreated();
        } );
    }
    
    private IEnumerator LoginProcess(BackendReturnObject bro, LoginType type)
    {
        if (!bro.IsSuccess())
        {
            PlayerPrefs.SetInt("LoginWay", -1);
            CheckLoginWayData();
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
                                Debug.Log("삭제된 계정입니다 토큰을 삭제합니다");
                                GuestIdDelete();
                            }
                            else if (bro.GetMessage().Contains("customId"))
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
            case LoginType.Custom:
                GetUserInfo();
                Debug.LogError("계정 로그인에 성공했습니다.");
                break;
            case LoginType.Auto:
                GetUserInfo();
                Debug.LogError("자동 로그인에 성공했습니다.");
                break;
        }
        userInfo.UserIndate = Backend.UserInDate;
        GetServerTime();
        DataManager.Instance.SetDefaultData();
        
        if (type == LoginType.Auto)
        {
            switch (bro.GetStatusCode())
            {
                case "201": //로그인
                    Debug.Log("자동 로그인 하여 서버에서 유저 데이터 불러오기 성공");
                    GetUserDataFromServer();
                    InsertLog(GameLogType.Login, $"{type}/{Application.version}");
                    break;
            }
        }
        else
        {
            switch (bro.GetStatusCode())
            {
                case "200": //로그인
                    Debug.Log("일반 로그인");
                    GetUserDataFromServer();
                    InsertLog(GameLogType.Login, $"{type}/{Application.version}");
                    break;
            }
        }
        DataManager.Instance.DataLoadComplete();
        
        StartCoroutine(nameof(RefreshToken));
    }

    public void CheckNickNameCreated()
    {
        if (Backend.UserNickName.Length <= 1)
        {
            UIManager.Instance.OpenPopup(UIManager.Instance.NickNamePrefab);
        }
        else
        {
            Backend.BMember.GetUserInfo((callback) =>
            {
                JsonData json = callback.GetReturnValuetoJSON()["row"];
                Debug.LogError(callback.GetReturnValue());
                Debug.Log(Backend.UserNickName + Backend.UserInDate);
                userInfo.UserIndate  = Backend.UserInDate;
                userInfo.Nickname    = Backend.UserNickName;
                userInfo.UID         = Backend.UID;
            
                MatchController.Instance.ChangeUI(1);
            });
        }
    }
    
    public void SetNewUserDataSaveToServer()
    {
        DataManager.Instance.SaveAllDataAtFirst();
    }
    public void GuestIdDelete()
    {
        if (Backend.BMember.GetGuestID().Length > 0)
        {
            Debug.LogFormat("GuestID {0} Delete", Backend.BMember.GetGuestID());
            Backend.BMember.DeleteGuestInfo();
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("LoginWay", -1);
        }
        else
        {
            Debug.LogFormat("Server Not Connected");
        }
    }
    public static void BackEndDebug(string str, BackendReturnObject bro)
    {
        Debug.Log(str);
        Debug.LogFormat("Status : {0}\nErrorCode : {1}\nMessage : {2}", bro.GetStatusCode(), bro.GetErrorCode(), bro.GetMessage());
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
                DataManager.Instance.SetLocalTime(parsedDate);
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
        CheckNickNameCreated();
    }
    
    [Header("전체 매치카드 리스트")]
    public List<MatchCard> allMatchCardList = new List<MatchCard>();

    void Update() {
        if (Backend.IsInitialized) {
            Backend.Match.Poll();
        }
    }
    
    public void JoinMatchMakingServer()
    {
         Debug.Log("서버접속 시도");

        Backend.Match.OnException = (Exception e) => { Debug.LogError(e.ToString()); };

        Backend.Match.OnJoinMatchMakingServer = (JoinChannelEventArgs args) => {
            Debug.Log(args.ErrInfo);
            
            if (args.ErrInfo == ErrorInfo.Success) {
                Debug.Log("1-2. OnJoinMatchMakingServer 성공");
            } else {
                Debug.LogError("1-2. OnJoinMatchMakingServer 실패");
            }
        };
        
        ErrorInfo errorInfo;
        
        if (Backend.Match.JoinMatchMakingServer(out errorInfo)) {
            Debug.Log("1-1. JoinMatchMakingServer 요청 : " + errorInfo.ToString());
        } else {
            Debug.LogError("1-1. JoinMatchMakingServer 에러 : " + errorInfo.ToString());
        }
    }

    public void CreateMatchRoom() {
        Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Success)
            {
                Debug.Log("2-2. OnMatchMakingRoomCreate 성공");

                if(isFastMatch)
                    RequestMatchMaking();
                else
                {
                    MatchController.Instance.ChangeUI(2);
                }

            } else {
                Debug.LogError("2-2. OnMatchMakingRoomCreate 실패");
            }
        };
        
        Debug.Log("2-1. CreateMatchRoom 요청");
        Backend.Match.CreateMatchRoom();
    }

    public void RequestMatchMaking()
    {
        if(!isFastMatch)
            FindTeamMatchCard(UserDataList.Count);

        isMatching = true;
        
        Backend.Match.OnMatchMakingResponse = (MatchMakingResponseEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Match_InProgress) {
                
                Debug.Log("3-2. OnMatchMakingResponse 매칭 신청 진행중");
                int second = allMatchCardList[matchIndex].transit_to_sandbox_timeout_ms / 1000;

                if (second > 0) {
                    Debug.Log($"{second}초 뒤에 샌드박스 활성화가 됩니다.");
                    StartCoroutine(WaitFor10Seconds(second));
                }

            } else if (args.ErrInfo == ErrorCode.Success) {
                Debug.Log("3-3. OnMatchMakingResponse 매칭 성사 완료");
                Debug.Log(args.MatchCardIndate);
                JoinGameServer(args.RoomInfo);
                isMatching = false;
            } else {
                Debug.LogError("3-2. OnMatchMakingResponse 매칭 신청 진행중 에러 발생 : " + args.ErrInfo + args.Reason + args.ToString());
                isMatching = false;
            }
        };
        
        Debug.Log("3-1. RequestMatchMaking 매칭 신청 시작");

        Debug.Log("매칭 신청정보 : " + allMatchCardList[matchIndex].matchType + "/" + allMatchCardList[matchIndex].matchModeType + "/" + allMatchCardList[matchIndex].inDate);
        Backend.Match.RequestMatchMaking(allMatchCardList[matchIndex].matchType, allMatchCardList[matchIndex].matchModeType, allMatchCardList[matchIndex].inDate);
    }
    
    IEnumerator WaitFor10Seconds(int second) {
        var delay = new WaitForSeconds(1.0f);
        for (int i = 0; i < second; i++)
        {
            if (!isMatching) break;
            
            Debug.Log($"{i}초 경과");
            yield return delay;
        }
    }

    public void GetMatchList()
    {
        allMatchCardList.Clear();

        Backend.Match.GetMatchList( callback => {
            if (!callback.IsSuccess()) {
                Debug.LogError("Backend.Match.GetMatchList Error : " + callback);
                return;
            }

            JsonData matchCardListJson = callback.FlattenRows();

            Debug.Log("Backend.Match.GetMatchList : " + callback);

            for (int i = 0; i < matchCardListJson.Count; i++) {
                MatchCard matchCard = new MatchCard();

                matchCard.inDate = matchCardListJson[i]["inDate"].ToString();

                matchCard.result_processing_type = matchCardListJson[i]["result_processing_type"].ToString();

                matchCard.version = Int32.Parse(matchCardListJson[i]["version"].ToString());

                matchCard.matchTitle = matchCardListJson[i]["matchTitle"].ToString();

                matchCard.enable_sandbox = matchCardListJson[i]["enable_sandbox"].ToString() == "true" ? true : false;

                string matchType = matchCardListJson[i]["matchType"].ToString();
                string matchModeType = matchCardListJson[i]["matchModeType"].ToString();

                switch (matchType) {
                    case "random":
                        matchCard.matchType = MatchType.Random;
                        break;

                    case "point":
                        matchCard.matchType = MatchType.Point;
                        break;

                    case "mmr":
                        matchCard.matchType = MatchType.MMR;
                        break;
                }

                matchCard.matchHeadCount = Int32.Parse(matchCardListJson[i]["matchHeadCount"].ToString());

                matchCard.enable_battle_royale = matchCardListJson[i]["enable_battle_royale"].ToString() == "true" ? true : false;

                matchCard.match_timeout_m = Int32.Parse(matchCardListJson[i]["match_timeout_m"].ToString());

                matchCard.transit_to_sandbox_timeout_ms = Int32.Parse(matchCardListJson[i]["transit_to_sandbox_timeout_ms"].ToString());

                matchCard.match_start_waiting_time_s = Int32.Parse(matchCardListJson[i]["match_start_waiting_time_s"].ToString());

                if (matchCardListJson[i].ContainsKey("match_increment_time_s")) {
                    matchCard.match_increment_time_s = Int32.Parse(matchCardListJson[i]["match_increment_time_s"].ToString());
                }

                if (matchCardListJson[i].ContainsKey("maxMatchRange")) {
                    matchCard.maxMatchRange = Int32.Parse(matchCardListJson[i]["maxMatchRange"].ToString());
                }

                if (matchCardListJson[i].ContainsKey("increaseAndDecrease")) {
                    matchCard.increaseAndDecrease = Int32.Parse(matchCardListJson[i]["increaseAndDecrease"].ToString());
                }

                if (matchCardListJson[i].ContainsKey("initializeCycle")) {
                    matchCard.initializeCycle = matchCardListJson[i]["initializeCycle"].ToString();
                }

                if (matchCardListJson[i].ContainsKey("defaultPoint")) {
                    matchCard.defaultPoint = Int32.Parse(matchCardListJson[i]["defaultPoint"].ToString());
                }

                if (matchCardListJson[i].ContainsKey("savingPoint")) {
                    if (matchCardListJson[i]["savingPoint"].IsArray) {
                        for (int listNum = 0; listNum < matchCardListJson[i]["savingPoint"].Count; listNum++) {
                            var keyList = matchCardListJson[i]["savingPoint"][listNum].Keys;
                            foreach (var key in keyList) {
                                matchCard.savingPoint.Add(key, Int32.Parse(matchCardListJson[i]["savingPoint"][listNum][key].ToString()));
                            }
                        }
                    } else {
                        foreach (var key in matchCardListJson[i]["savingPoint"].Keys) {
                            matchCard.savingPoint.Add(key, Int32.Parse(matchCardListJson[i]["savingPoint"][key].ToString()));
                        }
                    }
                }

                switch (matchModeType) {
                    case "OneOnOne":
                        matchCard.matchModeType = MatchModeType.OneOnOne;
                        break;

                    case "TeamOnTeam":
                        matchCard.matchModeType = MatchModeType.TeamOnTeam;
                        break;

                    case "Melee":
                        matchCard.matchModeType = MatchModeType.Melee;
                        break;

                    default :
                        matchCard.matchModeType = MatchModeType.Melee;
                        break;
                }
                
                allMatchCardList.Add(matchCard);
            }
        });
    }

    [Serializable]
    public class MatchCard {
        public string inDate;
        public string matchTitle;
        public bool enable_sandbox;
        public MatchType matchType;
        public MatchModeType matchModeType;
        public int matchHeadCount;
        public bool enable_battle_royale;
        public int match_timeout_m;
        public int transit_to_sandbox_timeout_ms;
        public int match_start_waiting_time_s;
        public int match_increment_time_s;
        public int maxMatchRange;
        public int increaseAndDecrease;
        public string initializeCycle;
        public int defaultPoint;
        public int version;
        public string result_processing_type;
        public Dictionary<string, int> savingPoint = new Dictionary<string, int>(); // 팀전/개인전에 따라 키값이 달라질 수 있음.

        public override string ToString() {
            string savingPointString = "savingPont : \n";
            foreach (var dic in savingPoint) {
                savingPointString += $"{dic.Key} : {dic.Value}\n";
            }

            savingPointString += "\n";
            return $"inDate : {inDate}\n" +
                   $"matchTitle : {matchTitle}\n" +
                   $"enable_sandbox : {enable_sandbox}\n" +
                   $"matchType : {matchType}\n" +
                   $"matchModeType : {matchModeType}\n" +
                   $"matchHeadCount : {matchHeadCount}\n" +
                   $"enable_battle_royale : {enable_battle_royale}\n" +
                   $"match_timeout_m : {match_timeout_m}\n" +
                   $"transit_to_sandbox_timeout_ms : {transit_to_sandbox_timeout_ms}\n" +
                   $"match_start_waiting_time_s : {match_start_waiting_time_s}\n" +
                   $"match_increment_time_s : {match_increment_time_s}\n" +
                   $"maxMatchRange : {maxMatchRange}\n" +
                   $"increaseAndDecrease : {increaseAndDecrease}\n" +
                   $"initializeCycle : {initializeCycle}\n" +
                   $"defaultPoint : {defaultPoint}\n" +
                   $"version : {version}\n" +
                   $"result_processing_type : {result_processing_type}\n" +
                   savingPointString;
        }
    }
    
    [Serializable]
    public class UserInfo
    {
        public string UserIndate  = string.Empty;
        public string Nickname    = string.Empty;
        public string UID         = string.Empty;
    }
    
    MatchInGameRoomInfo currentGameRoomInfo;
    Dictionary<string, MatchUserGameRecord> inGameUserList = new Dictionary<string, MatchUserGameRecord>();

    public void LeaveMatchMaking() {
        Backend.Match.OnLeaveMatchMakingServer = (LeaveChannelEventArgs args) => {
            Debug.Log("OnLeaveMatchMakingServer - 매칭 서버 접속 종료 : " + args.ToString());
        };
        
        Debug.Log($"5-a. LeaveMatchMakingServer 매치메이킹 서버 접속 종료 요청");
        
        Backend.Match.LeaveMatchMakingServer();
        
        isFastMatch = false;

        isMatching = false;
        
        matchIndex = 0;
    }

    public void JoinGameServer(MatchInGameRoomInfo gameRoomInfo) {
        
        Backend.Match.OnSessionJoinInServer += (args) => {
            LeaveMatchMaking();
            JoinGameRoom();
            SceneManager.LoadScene(1);
        };
        
        // Backend.Match.OnSessionJoinInServer = (JoinChannelEventArgs args) => {
        //     if (args.ErrInfo == ErrorInfo.Success) {
        //         Debug.Log("4-2. OnSessionJoinInServer 게임 서버 접속 성공 : " + args.ToString());
        //         Debug.Log("이제 게임방에 접속할 수 있습니다!");
        //     } else {
        //         Debug.LogError("4-2. OnSessionJoinInServer 게임 서버 접속 실패 : " + args.ToString());
        //         JoinGameServer(gameRoomInfo);
        //     }
        //     
        //     // 게임 서버에 정상적으로 접속했으면 매칭 서버를 종료
        //
        // };

        Debug.Log("4-1. JoinGameServer 인게임 서버 접속 요청");
        
        currentGameRoomInfo = gameRoomInfo;
        ErrorInfo errorInfo = null;

        if (Backend.Match.JoinGameServer(currentGameRoomInfo.m_inGameServerEndPoint.m_address, currentGameRoomInfo.m_inGameServerEndPoint.m_port, false, out errorInfo) == false) {
            Debug.LogError("JoinGameServer 중 로컬 에러가 발생했습니다." + errorInfo);
        }
    }

    public void JoinGameRoom() {
        Backend.Match.OnSessionListInServer = (MatchInGameSessionListEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Success) {
                Debug.Log("5-2. OnSessionListInServer 게임룸 접속 성공 : " + args.ToString());

                foreach (var list in args.GameRecords) {
                    if (inGameUserList.ContainsKey(list.m_nickname)) {
                        continue;
                    }
                    inGameUserList.Add(list.m_nickname, list);

                    UserData userData = new UserData();

                    userData.playerName = list.m_nickname;
                    userData.playerToken = list.m_sessionId.ToString();
                    userData.isSuperGamer = list.m_isSuperGamer;

                    UserDataList.Add(userData);
                    
                    Debug.Log(args.GameRecords.Count + "명의 유저중" + UserDataList.Count + "접속 완료");
                }

            } else {
                Debug.LogError("5-2. OnSessionListInServer : " + args.ToString());
            }
        };

        Backend.Match.OnMatchInGameAccess = (MatchInGameSessionEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Success) {
                Debug.Log($"5-3. OnMatchInGameAccess - 했습니다 : {args.GameRecord.m_nickname}({args.GameRecord.m_sessionId})");
                if (!inGameUserList.ContainsKey(args.GameRecord.m_nickname))
                {
                    inGameUserList.Add(args.GameRecord.m_nickname, args.GameRecord);

                    UserData userData = new UserData();
                    
                    userData.playerName = args.GameRecord.m_nickname;
                    userData.playerToken = args.GameRecord.m_sessionId.ToString();
                    userData.isSuperGamer = args.GameRecord.m_isSuperGamer;

                    UserDataList.Add(userData);
                    
                    Debug.Log(UserDataList.Count + "명 접속 확인 됐음");
                }
            } else {
                Debug.LogError("5-3. OnMatchInGameAccess : " + args.ErrInfo.ToString());
            }
            for (int i = 0; i < UserDataList.Count; i++)
            {
                if (UserDataList[i].playerName == userInfo.Nickname)
                {
                    Debug.Log("참가자인 나는 슈퍼게이머인가? :" + UserDataList[i].isSuperGamer);
                    isMeSuperGamer = UserDataList[i].isSuperGamer;
                }
            }
        };
        
        Backend.Match.OnMatchInGameStart = () => {
            string userListString = "접속한 유저 : \n";
            foreach (var list in inGameUserList)
            {
                if (inGameUserList.ContainsKey(list.Value.m_nickname)) {
                    continue;
                }
                
                userListString += $"{list.Value.m_nickname}({list.Value.m_sessionId})" + (list.Value.m_isSuperGamer == true ? "슈퍼게이머" : "");

                UserData data = new UserData();
                    
                data.playerToken = list.Value.m_sessionId.ToString();
                data.playerName = list.Value.m_nickname;
                data.isSuperGamer = list.Value.m_isSuperGamer;
                
                UserDataList.Add(data);
            }
        };
        
        Debug.Log($"5-1. JoinGameRoom 게임룸 접속 요청 : 토큰({currentGameRoomInfo.m_inGameRoomToken}");
        Backend.Match.JoinGameRoom(currentGameRoomInfo.m_inGameRoomToken);
    }
    // 릴레이할 데이터
    public class Message {
        public string message;
        public int x;
        public int y;
        public bool imnotsupergamer;

        public override string ToString() {
            return message;
        }
    }
    
    public void SendData() {
        if ( Backend.Match.OnMatchRelay == null) {
            Backend.Match.OnMatchRelay = (MatchRelayEventArgs args) => {
                var strByte = System.Text.Encoding.Default.GetString(args.BinaryUserData);
                Message msg = JsonUtility.FromJson<Message>(strByte);
                Debug.Log($"서버에서 받은 데이터 : {args.From.NickName} : {msg.ToString()}");
            };
        }
        
        Message message = new Message();
        //message.message = _inputField.text;
        var jsonData = JsonUtility.ToJson(message); // 클래스를 json으로 변환해주는 함수
        var dataByte = System.Text.Encoding.UTF8.GetBytes(jsonData); // json을 byte[]로 변환해주는 함수
        Backend.Match.SendDataToInGameRoom(dataByte);
    }

    public void MatchEnd() {
        Backend.Match.OnLeaveInGameServer = (MatchInGameSessionEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Success) {
                Debug.Log("OnLeaveInGameServer 인게임 서버 접속 종료 : " + args.ErrInfo.ToString());
            } else {
                Debug.LogError("OnLeaveInGameServer 인게임 서버 접속 종료 : " + args.ErrInfo + " / " + args.Reason);
            }
        };
        
        Backend.Match.OnMatchResult = (MatchResultEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Success) {
                Debug.Log("8-2. OnMatchResult 성공 : " + args.ErrInfo.ToString());
            } else {
                Debug.LogError("8-2. OnMatchResult 실패 : " + args.ErrInfo.ToString());
            }
        };        
        Debug.Log("8-1. MatchEnd 호출");
        MatchGameResult matchGameResult = new MatchGameResult();
        matchGameResult.m_winners = new List<SessionId>();
        matchGameResult.m_losers = new List<SessionId>();
        
        foreach (var session in inGameUserList) {
            // 순서는 무관합니다.
            matchGameResult.m_winners.Add(session.Value.m_sessionId);
        }
        
        Backend.Match.MatchEnd(matchGameResult);
    }
    
    public void AddTransactionInsert(UserDataType table, Param param)
    {
        for (int i = 0; i < transactionList.Count; i++)
            if (transactionList[i].table.ToString() == table.ToString())
                transactionList.RemoveAt(i);
        transactionList.Add(TransactionValue.SetInsert(table.ToString(), param));
        if (transactionList.Count > 9)
            SendTransaction(TransactionType.Insert);
    }
    
    public void GetUserDataFromServer()
    {
        transactionList.Clear();
        for(int i = 0; i < Enum.GetValues(typeof(UserDataType)).Length; i++)
            AddTransactionSetGet((UserDataType)i);
        SendTransaction(TransactionType.SetGet, DataManager.Instance.userData);
    }
    
    public void AddTransactionSetGet(UserDataType table)
    {
        for (int i = 0; i < transactionList.Count; i++)
            if (transactionList[i].table.ToString() == table.ToString())
                transactionList.RemoveAt(i);
        transactionList.Add(TransactionValue.SetGet(table.ToString(), new Where()));
        if (transactionList.Count > 9)
            SendTransaction(TransactionType.SetGet);
    }
    
    public BackendReturnObject SendTransaction(TransactionType type)
    {
         if(transactionList.Count <= 0) 
            return null;
         
         BackendReturnObject bro = null;
        
        switch (type)
        {
            case TransactionType.Insert:
            case TransactionType.Update:
                bro = Backend.GameData.TransactionWriteV2(transactionList);
                break;
            case TransactionType.SetGet:
                bro = Backend.GameData.TransactionReadV2(transactionList);
                break;
        }
        
        if (bro.IsSuccess())
        {
            switch (type)
            {
                case TransactionType.Insert:
                    BackendReturnObject broInsert = Backend.GameData.TransactionWriteV2(transactionList);
                    if (broInsert.IsSuccess())
                    {
                        JsonData json = broInsert.GetReturnValuetoJSON()["putItem"];
                        for (int i = 0; i < json.Count; i++)
                            DataManager.Instance.SetRowInDate((UserDataType)Enum.Parse(typeof(UserDataType), json[i]["table"].ToString()), json[i]["inDate"].ToString());
                        SuccessLoadDataCount += json.Count;
                    }
                    break;
                case TransactionType.SetGet:
                    JsonData gameDataListJson = bro.GetFlattenJSON()["Responses"];
                    for (int j = 0; j < gameDataListJson.Count; j++)
                        DataManager.Instance.SetUserData(Enum.Parse<UserDataType>(transactionList[j].table), gameDataListJson[j]);
                    break;
                case TransactionType.Update:
                    for (int i = 0; i < transactionList.Count; i++)
                    {
                        Param param = new Param();
                        param = transactionList[i].param;
                        string paramToString = JsonConvert.SerializeObject(param.GetValue());
                        int count = System.Text.Encoding.Default.GetByteCount(paramToString);
                        Debug.Log($"{transactionList[i].table} / byte : {count}");
                    }                    
                    Debug.LogError($"Data Update Success");
                    break;
            }
        }
        else
        {
            Debug.LogError($"Send Failed {bro.IsSuccess()} {bro.GetStatusCode()} {bro.GetErrorCode()} {bro.GetMessage()}");
        }
        transactionList.Clear();
        return bro;
    }
    
    public BackendReturnObject SendTransaction(TransactionType type, object obj)
    {

        if (transactionList.Count <= 0) {
            return null;
        }

        BackendReturnObject bro = null;
        switch (type)
        {
            case TransactionType.Insert:
                BackendReturnObject broInsert = Backend.GameData.TransactionWriteV2(transactionList);
                if (broInsert.IsSuccess())
                {
                    JsonData json = broInsert.GetReturnValuetoJSON()["putItem"];
                    for (int i = 0; i < json.Count; i++)
                        DataManager.Instance.SetRowInDate((UserDataType)Enum.Parse(typeof(UserDataType), json[i]["table"].ToString()), json[i]["inDate"].ToString());
                    SuccessLoadDataCount += json.Count;
                }
                else
                {
                    Debug.LogError($"Send Failed {broInsert.IsSuccess()} {broInsert.GetStatusCode()} {broInsert.GetErrorCode()} {broInsert.GetMessage()}");
                }
                break;
            case TransactionType.Update:
                for (int i = 0; i < transactionList.Count; i++)
                {
                    Param param = new Param();
                    param = transactionList[i].param;
                    string paramToString = JsonConvert.SerializeObject(param.GetValue());
                    int count = System.Text.Encoding.Default.GetByteCount(paramToString);
                    Debug.Log($"{transactionList[i].table} / byte : {count}");
                }
                Backend.GameData.TransactionWriteV2(transactionList, (callback) =>
                {
                    if (callback.IsSuccess())
                    {
                        Debug.Log($"Data Update Success");
                    }
                    else
                    {
                        if (callback.GetStatusCode().Contains("400"))
                        {
                            if (callback.GetErrorCode().Contains("HttpRequestException"))
                            {
                                //네트워크 연결 끊어짐
                            }
                        }

                        if (callback.GetStatusCode().Contains("401"))
                        {
                            if (callback.GetErrorCode().Contains("BadUnauthorizedException"))
                            {
                                //업데이트 에러
                            }
                        }
                        Debug.LogError($"Send Failed {callback.IsSuccess()} {callback.GetStatusCode()} {callback.GetErrorCode()} {callback.GetMessage()}");
                    }
                });
                break;
            case TransactionType.SetGet:
                List<TransactionValue> actions = new List<TransactionValue>();
                actions = new(transactionList);
                Backend.GameData.TransactionReadV2(actions, (callback) =>
                {
                    // 이후 처리
                    if (callback.IsSuccess())
                    {
                        JsonData gameDataListJson = callback.GetFlattenJSON()["Responses"];
                        for (int j = 0; j < gameDataListJson.Count; j++)
                            DataManager.Instance.SetUserData(Enum.Parse<UserDataType>(actions[j].table), gameDataListJson[j]);
                        SuccessLoadDataCount += gameDataListJson.Count;
                    }
                    else
                    {
                        switch (callback.GetStatusCode())
                        {
                            case "404":
                                if (callback.GetErrorCode() == "NotFoundException")
                                {
                                    transactionList.Clear();
                                    for (int i = 0; i < actions.Count; i++)
                                    {
                                        BackendReturnObject bro = Backend.GameData.GetMyData(actions[i].table, new Where());
                                        if (bro.IsSuccess())
                                        {
                                            JsonData data = bro.FlattenRows();
                                            if (data.Count <= 0)
                                            {
                                                //insert 필요
                                                switch (actions[i].table)
                                                {
                                                    case "UserBattleInfo":
                                                        DataManager.Instance.SaveUserBattleInfo(ServerSaveType.Insert);
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                DataManager.Instance.SetUserData(Enum.Parse<UserDataType>(actions[i].table), data[0]);
                                                SuccessLoadDataCount++;
                                            }
                                        }
                                        else
                                        {
                                            Debug.LogError($"GetData {bro.IsSuccess()} {bro.GetStatusCode()} {bro.GetErrorCode()} {bro.GetMessage()}");
                                        }
                                    }
                                    SendTransaction(TransactionType.Insert, DataManager.Instance.userData);
                                }
                                break;
                        }
                    }
                });
                break;
        }
        transactionList.Clear();
        return bro;
    }
    
    public void AddTransactionUpdate(UserDataType table, string indate, Param param)
    {
        for (int i = 0; i < transactionList.Count; i++)
            if (transactionList[i].table.ToString() == table.ToString())
                transactionList.RemoveAt(i);
        transactionList.Add(TransactionValue.SetUpdateV2(table.ToString(), indate, Backend.UserInDate,  param));
        if (transactionList.Count > 9)
            SendTransaction(TransactionType.Update, DataManager.Instance.userData);
    }

    public void FindID_WithEmail(string email)
    {
        Backend.BMember.FindCustomID( email, ( callback ) =>
        {
            
            switch (callback.GetStatusCode())
            {
                case "204" :
                    UIManager.Instance.PopupListPop();
                    UIManager.Instance.OpenRecyclePopup("안내", "입력하신 이메일로 전송 완료 했습니다.", null);
                    break;
                case "404" :
                    UIManager.Instance.OpenRecyclePopup("안내", "존재하지 않는 이메일 주소 입니다.", null);
                    break;
                case "429" :
                    UIManager.Instance.OpenRecyclePopup("경고", "요청 횟수를 초과 하셨습니다.", null);
                    break;
                case "400" :
                    UIManager.Instance.OpenRecyclePopup("Notice", "Project Name Error", null);
                    break;
            }
        });
    }

    public void ResetPW_WithEmailandID(string ID, string Email)
    {
        Backend.BMember.ResetPassword(ID, Email, (callback) =>
        {
            UIManager.Instance.PopupListPop();
            
            switch (callback.GetStatusCode())
            {
                case "204" :
                    UIManager.Instance.PopupListPop();
                    UIManager.Instance.OpenRecyclePopup("안내", "입력하신 이메일로 전송 완료 했습니다.", null);
                    break;
                case "404" :
                    UIManager.Instance.OpenRecyclePopup("안내", "존재하지 않는 이메일 주소 입니다.", null);
                    break;
                case "429" :
                    UIManager.Instance.OpenRecyclePopup("경고", "요청 횟수를 초과 하셨습니다.", null);
                    break;
                case "400" :
                    UIManager.Instance.OpenRecyclePopup("Notice", "Project Name Error", null);
                    break;
            }
        });
    }

    private void FindTeamMatchCard(int headCount)
    {
        int findValue = headCount * 2;

        for (int i = 0; i < allMatchCardList.Count; i++)
        {
            if (allMatchCardList[i].matchHeadCount == findValue && allMatchCardList[i].matchModeType == MatchModeType.TeamOnTeam)
            {
                matchIndex = i;
            }
        }
    }

    private void FindSoloMatchCard(int headCount)
    {
        
    }

    IEnumerator CheckSuperGamgerisMeCor()
    {
        UserData myData = new UserData();
        
        for (int i = 0; i < UserDataList.Count; i++)
        {
            if (UserDataList[i].playerName == userInfo.Nickname)
            {
                myData = UserDataList[i];
            }
        }

        if (myData.isSuperGamer)
        {
            Debug.Log("슈퍼게이머라서 대기합니다");
            yield return new WaitForSeconds(1);

            isLoadGame = true;
        }
        else
        {
            Debug.Log("슈퍼게이머가 아니라서 바로 로딩합니다");
            isLoadGame = true;
        }
    }
}


[Serializable]
public class RoomSettingData
{
    [Header("방설정 정보")]
    public int mapType;
    public int roomHeadCount;
    public int roomIndexNum;
        
    public string roomName;
}
