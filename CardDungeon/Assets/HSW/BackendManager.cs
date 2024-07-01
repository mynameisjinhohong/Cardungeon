using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Assets.SimpleGoogleSignIn;
using Assets.SimpleGoogleSignIn.Scripts;
using BackEnd;
using BackEnd.Tcp;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[Serializable]
public class InGameUserDataDic : SerializableDictionary<string, MatchUserGameRecord> { }

public class BackendManager : Singleton<BackendManager>
{
    private static BackendManager instance;   // 인스턴스
    
    private Thread serverCheckThread;

    public ServerType serverType;
    public LoginType loginType;

    public int successLoadDataCount = 0;
    
    [Header("유저정보")]
    public UserInfo userInfo;
    
    public bool loadServerTime   = false;
    public int checkLoginWayData = -1;
    public bool isInitialize     = false;
    public bool isLogined        = false;
    public bool isLoadGame       = false;
    public bool UseAutoLogin     = false; 
    public int matchIndex        = 0;
    public bool isMatching       = false;
    public bool isMeSuperGamer   = false;

    public string winUser = "";

    [Header("전체 유저 데이터 리스트")] 
    public List<UserData> userDataList;

    public bool isFastMatch;

    // 게임 종료후 메인 화면에서 플레이 했던 유저인지 체크하는 값
    public bool isPlayedUser;
    public bool isEscapeWin;

    public string inviterName;
    
    private int initTimeCount = 0;

    [SerializeField]
    List<TransactionValue> transactionList = new List<TransactionValue>();
    
    private Coroutine sandBoxMatchWaitCor;
    private Coroutine matchLimitTimeCor;

    private int currentSceneIndex;
    void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        if(!isInitialize)
            Initialize();

        if (currentSceneIndex == 0)
        {
            
        }
    }
    
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        // 모든 씬에서 유지
        DontDestroyOnLoad(gameObject);
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
            settings.clientAppID     = "3cde6b10-2e77-11ef-9300-214337633e707823";
            settings.signatureKey    = "3cde6b11-2e77-11ef-9300-214337633e707823";
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

            UIManager.Instance.OpenRecyclePopup("네트워크 에러", $"에러코드{bro.GetErrorCode()}\n고객센터로 문의해주세요", Application.Quit);
        }
        
    }

    public void ResetInGameData()
    {
        isLoadGame     = false;
        isLogined      = false;
        isMatching     = false;
        isMeSuperGamer = false;
        isFastMatch    = false;
        isPlayedUser   = false;
        
        matchIndex = -1;

        inviterName = "";
        winUser = "";

        inGameUserList.Clear();
        userDataList.Clear();
        userGradeList.Clear();
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
    
    public void GoogleLoginSetting()
    {
#if PLATFORM_ANDROID
                // GPGS 플러그인 설정
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
                .Builder()
            .RequestServerAuthCode(false)
            .RequestEmail() // 이메일 권한을 얻고 싶지 않다면 해당 줄(RequestEmail)을 지워주세요.
            .RequestIdToken()
            .Build();
        //커스텀 된 정보로 GPGS 초기화
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true; // 디버그 로그를 보고 싶지 않다면 false로 바꿔주세요.
        //GPGS 시작.
        PlayGamesPlatform.Activate();
        
        GoogleLoginSequense();
#endif
    }
    
    public void GoogleLoginSequense()
    {
        if (Social.localUser.authenticated == true)
        {
            BackendReturnObject bro = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");
            Debug.LogError($"{bro.IsSuccess()} {bro.GetStatusCode()} {bro.GetErrorCode()} {bro.GetMessage()}");
            if(bro.IsSuccess())
                StartCoroutine((LoginProcess(bro, LoginType.Google)));
        }
        else
        {
            Social.localUser.Authenticate((bool success) => {
                if (success)
                {
                    BackendReturnObject bro = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");
                    Debug.LogError($"{bro.IsSuccess()} {bro.GetStatusCode()} {bro.GetErrorCode()} {bro.GetMessage()}");
                    if (bro.IsSuccess())
                    {
                        StartCoroutine(LoginProcess(bro, LoginType.Google));
                        PlayerPrefs.SetInt("LoginWay", 1);
                    }
                }
                else
                {
                    // ?????? ????
                    Debug.Log("Login failed for some reason");
                }
            });
        }
    }
    
    public string GetTokens()
    {
#if PLATFORM_ANDROID
                if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            string _IDtoken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
            return _IDtoken;
        }
        else
        {
            Debug.Log("Token get Failed. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
            return null;
        }
#endif
        return null;
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

    public void TryCustomSignin(string id, string pw, string email)
    {
        Backend.BMember.CustomSignUp ( id, pw, callback => {
            if(callback.IsSuccess())
            {
                Debug.Log("회원가입에 성공했습니다.");
                
                Backend.BMember.UpdateCustomEmail(email, ( callback ) =>
                {
                    UIManager.Instance.PopupListPop();
                
                    UIManager.Instance.PopupListPop();

                    // 닉네임 생성 팝업 만들기
                    UIManager.Instance.OpenPopup(UIManager.Instance.NickNamePrefab);

                    DataManager.Instance.SetUserDefaultData();

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
                Debug.Log("커스텀 계정으로 자동 로그인");
                Backend.BMember.LoginWithTheBackendToken((callback) =>
                {
                    if(PlayerPrefs.GetInt("LoginWay") == 0)
                        StartCoroutine(LoginProcess(callback, LoginType.Custom));
                });
                break;
            case 1 :
                Debug.Log("구글 계정으로 자동 로그인");
                Backend.BMember.LoginWithTheBackendToken((callback) =>
                {
                    if(PlayerPrefs.GetInt("LoginWay") == 1)
                        StartCoroutine(LoginProcess(callback, LoginType.Google));
                });
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
                Debug.LogError("커스텀 로그인, 데이터 입력");
                loginType = LoginType.Custom;
                break;
            case LoginType.Google:
                loginType = LoginType.Google;
                Debug.LogError("구글 로그인, 데이터 입력");
                break;
        }
        userInfo.UserIndate = Backend.UserInDate;
        GetServerTime();
        
        switch (bro.GetStatusCode())
        {
            case "200": //로그인
                Debug.Log("기존 유저 로그인 성공");
                InsertLog(GameLogType.Login, $"{type}/{Application.version}");
                break;
            case "201": //로그인
                Debug.Log("신규 유저 로그인 성공");
                InsertLog(GameLogType.Login, $"{type}/{Application.version}");
                // 구글 로그인 초회 실행시 오토 로그인 
                PlayerPrefs.SetInt("UseAutoLogin", 1);
                break;
        }
        
        //푸시 알림 수신 하는 부분
        //SetupManager.Instance.RegistPush();();
        
        StartCoroutine(nameof(RefreshToken));
        
        CheckNickNameCreated();
    }


    public void CheckNickNameCreated()
    {
        if (Backend.UserNickName.Length <= 1)
        {
            UIManager.Instance.OpenPopup(UIManager.Instance.NickNamePrefab);
        }
        else
        {
            Debug.Log("데이터 서버로부터 수신 후 메인씬으로 이동 요청");
            GetUserInfo();
        }
    }

    public void GetUserInfo()
    {
        Backend.BMember.GetUserInfo((callback) =>
        {
            isLogined = true;
            
            JsonData json = callback.GetReturnValuetoJSON()["row"];
            Debug.LogError(callback.GetReturnValue());
            Debug.Log(Backend.UserNickName + Backend.UserInDate);
            userInfo.UserIndate  = Backend.UserInDate;
            userInfo.Nickname    = Backend.UserNickName;
            userInfo.UID         = Backend.UID;
            
            GetUserDataFromServer();
            
            MatchController.instance.ChangeUI(1);
        });
    }
    
    // 로그인 완료시 발생하는 이벤트들 초회 실행 후 씬이동 이후에도 잘돌아가는지 확인
    public void MainUIEvents()
    {
        Debug.Log("메인UI 이벤트 모두 동작 실행");
        
        // 방만들기 누를시
        Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Success)
            {
                Debug.Log("2-2. OnMatchMakingRoomCreate 성공");

                if (!isFastMatch)
                {
                    Debug.Log("커스텀 매칭 UI로 이동");
                    MatchController.instance.ChangeUI(2);
                }
                else
                {
                    Debug.Log("빠른 매칭 실행");
                    TryMatch();
                }
            }
            else
            { 
                Debug.LogError($"$2-2.{args.Reason}로 인해 OnMatchMakingRoomCreate 실패\n에러코드 : {args.ErrInfo}");
                Backend.Match.LeaveMatchRoom();
            }
        };
        
        // 커스텀 매치에서 추가 인원 접속체크
        Backend.Match.OnMatchMakingRoomJoin = (MatchMakingGamerInfoInRoomEventArgs args) => {
            // 본인 제외
            if (args.UserInfo.m_nickName == userInfo.Nickname) return;
                
            Debug.Log(args.UserInfo.m_nickName + "님이 매칭방 접속");
            Debug.Log(args.ErrInfo + args.Reason);
                
            UserData getdata = new UserData();

            getdata.playerName = args.UserInfo.m_nickName;
            getdata.playerToken = args.UserInfo.m_sessionId.ToString();
            getdata.isSuperGamer = false;
                
            userDataList.Add(getdata);

            MatchController.instance.DataInit();
        };
        
        // 커스텀 매치에서 나가는 인원 이벤트
        Backend.Match.OnMatchMakingRoomLeave = (MatchMakingGamerInfoInRoomEventArgs args) => {
            Debug.Log(args.UserInfo.m_nickName + "님이 나감");

            int leaveUserIndex = 0;
                
            for (int i = 0; i < BackendManager.Instance.userDataList.Count; i++)
            {
                if (BackendManager.Instance.userDataList[i].playerName == args.UserInfo.m_nickName)
                {
                    leaveUserIndex = i;
                        
                    BackendManager.Instance.userDataList.RemoveAt(leaveUserIndex);
                }
            }
                
            MatchController.instance.DataInit();
        };

        // 커스텀 매치 입장시 방에 있는 유저 정보 로드 이벤트
        Backend.Match.OnMatchMakingRoomUserList = (MatchMakingGamerInfoListInRoomEventArgs args) => {

            foreach (var userInfo in args.UserInfos)
            {
                UserData getdata = new UserData();

                getdata.playerName = userInfo.m_nickName;
                getdata.playerToken = userInfo.m_sessionId.ToString();
                getdata.isSuperGamer = false;
                    
                BackendManager.Instance.userDataList.Add(getdata);
            }

            MatchController.instance.DataInit();
        };
        
        // 매칭 종료 이벤트
        Backend.Match.OnLeaveMatchMakingServer = (LeaveChannelEventArgs args) => {
            Debug.Log("OnLeaveMatchMakingServer - 매칭 서버 접속 종료 : " + args.ToString());
            
            Debug.Log($"5-a. LeaveMatchMakingServer 매치메이킹 서버 접속 종료 요청");

            isFastMatch = false;

            isMatching = false;
            
            if(!isPlayedUser)
                MatchController.instance.ChangeUI(1);
        };
        
        // 매칭 결과 이벤트
        Backend.Match.OnMatchMakingResponse = (MatchMakingResponseEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Match_InProgress) {

                MatchController.instance.ChangeUI(3);
                
                Debug.Log("3-2. OnMatchMakingResponse 매칭 신청");

                // 샌드박스는 커스텀 매칭만 사용
                MatchController.instance.timerText.gameObject.SetActive(isFastMatch);
                
                if (!isFastMatch)
                {
                    int second = allMatchCardList[matchIndex].transit_to_sandbox_timeout_ms / 1000;
                    
                    if (second > 0) {
                        Debug.Log($"{second}초 뒤에 샌드박스 활성화가 됩니다.");

                        if (sandBoxMatchWaitCor != null)
                        {
                            StopCoroutine(sandBoxMatchWaitCor);
                        }

                        sandBoxMatchWaitCor = StartCoroutine(WaitFor10Seconds(second));
                    }
                    
                    userDataList.Clear();
                    
                    JoinGameServer(args.RoomInfo);
                }
                else
                {
                    int minute = allMatchCardList[matchIndex].match_timeout_m;

                    if (matchLimitTimeCor != null)
                    {
                        StopCoroutine(matchLimitTimeCor);
                    }
                    
                    matchLimitTimeCor = StartCoroutine(WaitForMatchLimitTime(minute));
                }
            } else if (args.ErrInfo == ErrorCode.Success) {
                Debug.Log("3-3. OnMatchMakingResponse 매칭 성사 완료");
                JoinGameServer(args.RoomInfo);

            }
            else {

                Debug.LogError("3-2. OnMatchMakingResponse 매칭 신청 진행중 에러 발생 : " + args.ErrInfo + args.Reason + args.ToString());
                UIManager.Instance.OpenRecyclePopup("시스템 메세지", "매칭 신청 진행중 에러 발생" + args.ErrInfo, null);
            }
        };
        
        // 초대 오면 팝업 생성 이벤트
        Backend.Match.OnMatchMakingRoomSomeoneInvited = (args) => {
            UIManager.Instance.OpenInvitePopup(args.InviteUserInfo.m_nickName, args.RoomId, args.RoomToken);
        };
        
        // 매치 서버 접속 완료되면 발생 이벤트
        Backend.Match.OnJoinMatchMakingServer = (JoinChannelEventArgs args) => {
            Debug.Log(args.ErrInfo);
            
            if (args.ErrInfo == ErrorInfo.Success) {
                Debug.Log("1-2. OnJoinMatchMakingServer 성공");
            } else {
                if (args.ErrInfo.Category == ErrorCode.AuthenticationFailed)
                {
                    UIManager.Instance.OpenRecyclePopup("네트워크 에러", "다른 기기로 로그인이 확인되어 종료 합니다.", Application.Quit);
                }
                else
                {
                    Debug.LogError($"1-2. 이유 : {args.ErrInfo.Reason} 카테고리 : {args.ErrInfo.Category} OnJoinMatchMakingServer 실패");
                }
            }
        };
        
        // 인게임 서버 접속 이벤트
        Backend.Match.OnSessionJoinInServer = (args) => {
            
            Debug.Log(args.ErrInfo.Category);
            Debug.Log(args.Session.NickName + "님이 인게임 서버 접속 요청");
            Debug.Log("4-1. JoinGameServer 인게임 서버 접속 요청");
            isPlayedUser = true;
            
            LeaveMatchMaking();
            JoinGameRoom();
        };
        
        // 인게임에 접속자 발생시 이벤트
        Backend.Match.OnMatchInGameAccess = (MatchInGameSessionEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Success) {
                Debug.Log($"5-3. OnMatchInGameAccess - 했습니다 : {args.GameRecord.m_nickname}({args.GameRecord.m_sessionId})");
                
                //추가로 접속된 유저가 이미 리스트에 있는지 확인 후 리스트에 정보 추가
                if (!inGameUserList.ContainsKey(args.GameRecord.m_nickname))
                {
                    inGameUserList.Add(args.GameRecord.m_nickname, args.GameRecord);
                    
                    UserData userData = new UserData();
                    
                    userData.playerName = args.GameRecord.m_nickname;
                    userData.playerToken = args.GameRecord.m_sessionId.ToString();
                    userData.isSuperGamer = args.GameRecord.m_isSuperGamer;

                    userDataList.Add(userData);

                    // 전체 유저의 수
                    int CheckHeadCount = allMatchCardList[matchIndex].matchModeType != MatchModeType.TeamOnTeam
                        ? allMatchCardList[matchIndex].matchHeadCount
                        : (allMatchCardList[matchIndex].matchHeadCount / 2);
                    
                    if (CheckHeadCount <= userDataList.Count)
                    {
                        isPlayedUser = true;
                
                        SceneManager.LoadScene(1);
                    }
                }
            } else {
                Debug.LogError("5-3. OnMatchInGameAccess : " + args.ErrInfo.ToString());
            }
        };
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
                // DateTime parsedDate = DateTime.Parse(time);
                // DataManager.Instance.SetLocalTime(parsedDate);
                loadServerTime = true;
            }
            else
            {
                initTimeCount++;
                InitTime();
            }
        });
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
        ErrorInfo errorInfo;
        
        if (Backend.Match.JoinMatchMakingServer(out errorInfo)) {
            Debug.Log("1-1. JoinMatchMakingServer 요청 : " + errorInfo.ToString());

            if (isLogined)
            {
                MainUIEvents();
            }
            else
            {
                Debug.LogError("로그인 확인 실패, 메인UI 이벤트 실행 실패");
            }
            
        } else {
            Debug.LogError("1-1. JoinMatchMakingServer 에러 : " + errorInfo.ToString());
        }
    }
    
    public void CreateMatchRoom() {
        
        Debug.Log("2-1. CreateMatchRoom 요청");
        
        Backend.Match.CreateMatchRoom();
    }
    
    // 초대해서 시작하는 경우 매칭 요청
    public void RequestMatchMaking()
    {
        if (userDataList.Count <= 1)
        {
            UIManager.Instance.OpenRecyclePopup("시스템 메세지", "커스텀 매치는 2인 이상 부터\n플레이 가능합니다.", null);
            return;
        }
        
        if (allMatchCardList[matchIndex].matchHeadCount / 2 == userDataList.Count)
        {
            TryMatch();
        }
    }

    // 빠른 매칭 요청 ( 인원별 2, 4, 8 )
    public void RequestFastMatchMaking(int headCount)
    {
        FindFastMatchCard(headCount);
        
        isFastMatch = true;
        
        CreateMatchRoom();
    }

    public void TryMatch()
    {
        Debug.Log("3-1. RequestMatchMaking 매칭 신청 시작");
        
        isMatching = true;

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
    
    IEnumerator WaitForMatchLimitTime(int minute) {
        var delay = new WaitForSeconds(1.0f);

        int second = minute * 60;
        
        for (int i = 0; i < second; i++)
        {
            if (!isMatching) break;
            
            Debug.Log($"{i}초 경과");

            MatchController.instance.FormatTime(i);
            
            if (i >= (second - 1))
            {
                UIManager.Instance.OpenRecyclePopup("시스템 메세지", "매칭 실패 다시 시도 해주세요.", MatchController.instance.MatchingCancel);
                break;
            }
            yield return delay;
        }
    }

    public void GetMatchList()
    {
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
    
    // 유저가 나가도 보관하는 접속한 유저들의 리스트
    public InGameUserDataDic inGameUserList;
    
    public List<MatchUserGameRecord> userGradeList;
    
    public void LeaveMatchMaking() {
        
        Backend.Match.LeaveMatchMakingServer();
    }

    public void JoinGameServer(MatchInGameRoomInfo gameRoomInfo) {
        
        currentGameRoomInfo = gameRoomInfo;
        ErrorInfo errorInfo = null;
        
        if(Backend.Match.JoinGameServer(currentGameRoomInfo.m_inGameServerEndPoint.m_address, currentGameRoomInfo.m_inGameServerEndPoint.m_port, false, out errorInfo) == false)
        {
            // 에러 확인
            Debug.Log(errorInfo);
            return;
        }
        

        //Debug.LogError("JoinGameServer 중 로컬 에러가 발생했습니다." + errorInfo);
    }

    public void JoinGameRoom() {
        
        Backend.Match.JoinGameRoom(currentGameRoomInfo.m_inGameRoomToken);
        
        // 인게임 시작 대기 이벤트
        Backend.Match.OnMatchInGameStart = () => {
            Debug.Log($"5-1. JoinGameRoom 게임룸 접속 요청 : 토큰({currentGameRoomInfo.m_inGameRoomToken}");
        };
        
        // 전체 유저의 수
        int CheckHeadCount = allMatchCardList[matchIndex].matchModeType != MatchModeType.TeamOnTeam
            ? allMatchCardList[matchIndex].matchHeadCount
            : (allMatchCardList[matchIndex].matchHeadCount / 2);
        
        // 현재 연결된 게임룸의 유저 정보
        Backend.Match.OnSessionListInServer = (MatchInGameSessionListEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Success)
            {
                inGameUserList = new InGameUserDataDic();
                
                Debug.Log("5-2. OnSessionListInServer 게임룸 접속 성공 : " + args.ToString());

                Debug.Log(args.GameRecords.Count + "번째로 게임룸에 접속");

                foreach (var inGameUserData in args.GameRecords)
                {
                    inGameUserList.Add(inGameUserData.m_nickname, inGameUserData);
                    
                    UserData userData = new UserData();
                    
                    userData.playerName = inGameUserData.m_nickname;
                    userData.playerToken = inGameUserData.m_sessionId.ToString();
                    userData.isSuperGamer = inGameUserData.m_isSuperGamer;
                    
                    //이미 접속한 사람중 자신의 닉네임과 대조하여 슈퍼 게이머인지 확인
                    if (userData.playerName == userInfo.Nickname)
                    {
                        isMeSuperGamer = userData.isSuperGamer;
                    }
                    
                    userDataList.Add(userData);
                    
                    // 모든 플레이어 연결 성공시 인게임씬으로 이동
                    if (CheckHeadCount <= userDataList.Count)
                    {
                        SceneManager.LoadScene(1);
                    }
                }
            } else {
                Debug.LogError("5-2. OnSessionListInServer : " + args.ToString());
            }
        };
    }

    public void DoClassChoiceTime()
    {
        StartCoroutine(ClassChoiceTimeCor());
    }
    
    IEnumerator ClassChoiceTimeCor()
    {
        float waitTime = allMatchCardList[matchIndex].match_start_waiting_time_s;

        yield return new WaitForSeconds(waitTime);

        isLoadGame = true;

        GamePlayManager.Instance.CreateMap();
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

    public void SendResultToServer()
    {
        Debug.Log("8-1. MatchEnd 호출");
        MatchGameResult matchGameResult = new MatchGameResult();
        matchGameResult.m_winners = new List<SessionId>();
        matchGameResult.m_losers = new List<SessionId>();
        
        switch (allMatchCardList[matchIndex].matchModeType)
        {
            // 1:1 일때는 승자 패자 지정
            case MatchModeType.OneOnOne:
                foreach (var session in inGameUserList) {
                    if(session.Value.m_nickname == winUser)
                        matchGameResult.m_winners.Add(session.Value.m_sessionId);
                    else
                        matchGameResult.m_losers.Add(session.Value.m_sessionId);
                }
                break;
            // 팀전(커스텀 매치) 일때는 승자만 승자팀 나머지는 전부 패자 팀으로 처리 (순서 상관 X )
            case MatchModeType.TeamOnTeam:
                foreach (var session in inGameUserList) {
                    if(session.Value.m_nickname == winUser)
                        matchGameResult.m_winners.Add(session.Value.m_sessionId);
                    else
                        matchGameResult.m_losers.Add(session.Value.m_sessionId);
                }
                break;
            // 개인전 일떄는 1등부터 순서대로 처리
            case MatchModeType.Melee:

                if (isEscapeWin)
                {
                    // 열쇠 탈출하게 되면 탈출한사람 제외 패배처리
                    foreach (var session in inGameUserList) {
                        if(session.Value.m_nickname == winUser)
                            matchGameResult.m_winners.Add(session.Value.m_sessionId);
                        else
                            matchGameResult.m_losers.Add(session.Value.m_sessionId);
                    }
                }
                else
                {
                    for (int i = 0; i < userGradeList.Count; i++)
                    {
                        // 마지막 생존자를 1등으로 추가
                        userGradeList.Reverse();
                        matchGameResult.m_winners.Add(userGradeList[i].m_sessionId);
                    }
                }
                break;
            default :
                break;
        }
        
        Backend.Match.MatchEnd(matchGameResult);
        Debug.Log("게임 종료 요청 완료");
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
    
    public void AddTransactionUpdate(UserDataType table, string indate, Param param)
    {
        for (int i = 0; i < transactionList.Count; i++)
            if (transactionList[i].table.ToString() == table.ToString())
                transactionList.RemoveAt(i);
        transactionList.Add(TransactionValue.SetUpdateV2(table.ToString(), indate, Backend.UserInDate,  param));
        if (transactionList.Count > 9)
            SendTransaction(TransactionType.Update, DataManager.Instance.userBattleData);
    }

    public void AddTransactionSetGet(UserDataType table)
    {
        for (int i = 0; i < transactionList.Count; i++)
            if (transactionList[i].table == table.ToString())
                transactionList.RemoveAt(i);
        transactionList.Add(TransactionValue.SetGet(table.ToString(), new Where()));
        if (transactionList.Count > 9)
            SendTransaction(TransactionType.SetGet);
    }
    
    public void GetUserDataFromServer()
    {
        transactionList.Clear();
        for(int i = 0; i < Enum.GetValues(typeof(UserDataType)).Length; i++)
            AddTransactionSetGet((UserDataType)i);
        SendTransaction(TransactionType.SetGet, this);
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
                        successLoadDataCount += json.Count;
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
                    successLoadDataCount += json.Count;
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
                        successLoadDataCount += gameDataListJson.Count;
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
                                                    case "UserBattleData":
                                                        DataManager.Instance.SaveUserBattleData(ServerSaveType.Insert);
                                                        break;
                                                    case "UserInvenData":
                                                        DataManager.Instance.SaveUserInvenData(ServerSaveType.Insert);
                                                        break;
                                                    case "UserAchievementData":
                                                        DataManager.Instance.SaveUserAchievementData(ServerSaveType.Insert);
                                                        break;
                                                    case "DailyData":
                                                        DataManager.Instance.SaveDailyData(ServerSaveType.Insert);
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                DataManager.Instance.SetUserData(Enum.Parse<UserDataType>(actions[i].table), data[0]);
                                                successLoadDataCount++;
                                            }
                                        }
                                        else
                                        {
                                            Debug.LogError($"GetData {bro.IsSuccess()} {bro.GetStatusCode()} {bro.GetErrorCode()} {bro.GetMessage()}");
                                        }
                                    }
                                    SendTransaction(TransactionType.Insert, DataManager.Instance.userBattleData);
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

    public void FindTeamMatchCard(int headCount)
    {
        int findValue = headCount * 2;

        //팀매치 카드와 인원 * 2로 조회
        for (int i = 0; i < allMatchCardList.Count; i++)
        {
            if (allMatchCardList[i].matchHeadCount == findValue &&
                allMatchCardList[i].matchModeType == MatchModeType.TeamOnTeam)
            {
                matchIndex = i;
            }
        }
    }

    private void FindFastMatchCard(int headCount)
    {
        for (int i = 0; i < allMatchCardList.Count; i++)
        {
            // 일반전 2, 4 8명과 포인트 조건 제외, 개인전을 조건으로 탐색
            if (allMatchCardList[i].matchHeadCount == headCount &&
                allMatchCardList[i].matchType      == MatchType.Random &&
                allMatchCardList[i].matchModeType  != MatchModeType.TeamOnTeam)
            {
                matchIndex = i;
            }
        }
    }

    public void GoogleSignOut()
    {
        //GoogleSignIn.DefaultInstance.SignOut();
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
