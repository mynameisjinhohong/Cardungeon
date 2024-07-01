using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using BackEnd.Tcp;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class MatchController : MonoBehaviour
{
    public static MatchController instance;
    
    private BackendManager _backendManager;
    
    public List<GameObject> uIList;
    
    [Header("로그인")]
    public GameObject readyToPlay;

    public Button loginCheckButton;
    
    public GameObject LoginButtonListObj;

    public Button googleLoginBtn;
    
    [Header("메인 화면")]
    public TextMeshProUGUI userNickNameText;
    
    public GameObject Rabbit1;
    public GameObject Rabbit2;

    [Header("커스텀 매칭 룸")]
    public Transform DataPanelParent;
    
    public GameObject userInfoDataPanelObj;
    
    public GameObject emptyDataPanelObj;
    
    public GameObject tutorialPanelObj;

    public TextMeshProUGUI userCount;
    
    public TextMeshProUGUI roomNameText;

    public Button btn_Invite;

    public Button btn_MatchStart;

    [Header("매칭 대기")]
    public VideoPlayer matchingVideo;
    
    public TextMeshProUGUI TipText;

    public List<String> TipStrings;

    public TextMeshProUGUI timerText;

    //코루틴 관리
    private Coroutine blinkCoroutine;
    private Coroutine matchTextCoroutine;
    
    private void Awake()
    {
        instance = this;
        
        _backendManager = BackendManager.Instance;
    }

    public void Start()
    {
        // 일단 윈도우 버전은 구글 로그인 막아둡니다.
        #if PLATFORM_ANDROID
        googleLoginBtn.interactable = true;
        #endif
        googleLoginBtn.interactable = false;

        
        StartCoroutine(WaitInitDataCor());

        TipStrings.Add("뒤끝 서버가\n토끼들의 성장을 돕고있어요");
        TipStrings.Add("잠시만 기다려 주세요\n토끼들을 모아 훈련하는 중입니다.");
        TipStrings.Add("던전은 매번\n랜덤으로 생성됩니다.");
        TipStrings.Add("열쇠는 나무 상자나\n보유 중인 유저에게 얻을 수 있어요.");
        TipStrings.Add("열쇠를 얻었다면\n던전의 중심으로 달려가세요!");
        TipStrings.Add("기본 이동 카드는 MP 소모가 없습니다.\n얼마든지 깡총깡총!");
        TipStrings.Add("맵 곳곳에 새로운 카드들이 흩어져 있습니다.\n먼저 획득해서 강해지세요!");
        TipStrings.Add("카드에 쓰이는 MP는\n일정 시간이 지나면 충전됩니다.");
        TipStrings.Add("공격과 스킬 카드에 쓰이는\nMP는 일정 시간이 지나면 충전됩니다.");
        TipStrings.Add("'카드버리기'가 노란색이\n아닐때는 MP가 소모됩니다.");
        TipStrings.Add("나무 상자에는 행운 혹은 불행이 들어 있어요.\n운을 시험해 보세요!");
        TipStrings.Add("다른 유저가 먼저 탈출 하거나\nHP를 전부 잃으면 게임오버 됩니다.");
    }
    
    public void TryAutoLogin()
    {
        if(_backendManager.checkLoginWayData != -1 && BackendManager.Instance.UseAutoLogin)
        {
            Debug.Log("자동로그인 실행");
            _backendManager.StartTokenLogin();
        }
        else
        {
            Debug.Log("자동 로그인 꺼진 상태");
            TryLogin();
        }
    }

    public void TryLogin()
    {
        readyToPlay.gameObject.SetActive(false);
        LoginButtonListObj.gameObject.SetActive(true);
        loginCheckButton.enabled = false;
    }

    public void ChangeUI(int index)
    {
        UIManager.Instance.AllPopupClear();

        for (int i = 0; i < uIList.Count; i++)
        {
            uIList[i].SetActive(i == index);
        }

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        
        if (index == 1)
        {
            BackendManager.Instance.userDataList.Clear();
            
            userNickNameText.text = BackendManager.Instance.userInfo.Nickname;
            
            blinkCoroutine = StartCoroutine(RabbitBlinkEye());
            
            if(PlayerPrefs.GetInt("isFirstPlay") != 1)
            {
                UIManager.Instance.OpenPopup(tutorialPanelObj);
            }
            
            if (!_backendManager.isMatching)
            {
                _backendManager.GetMatchList();
                
                _backendManager.JoinMatchMakingServer();
            }
        }
        
        if (index == 3)
        {
            if(matchTextCoroutine != null)
                StopCoroutine(matchTextCoroutine);
            
            matchTextCoroutine = StartCoroutine(RandomTipTextCor());
            
            if(!matchingVideo.isPlaying)
                matchingVideo.Play();
        }
    }

    public void FormatTime(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    public void DataInit()
    {
        if (BackendManager.Instance.isPlayedUser) return;
        
        foreach (Transform child in DataPanelParent)
        {
            Destroy(child.gameObject);
        }
        
        for (int i = 0; i < 5; i++)
        {
            if (i < BackendManager.Instance.userDataList.Count)
            {
                GameObject userDataPanel = Instantiate(userInfoDataPanelObj, DataPanelParent);

                userDataPanel.GetComponent<UI_UserIDPanel_PCI>().txt_userName.text =
                    BackendManager.Instance.userDataList[i].playerName;
            }
            else
            {
                Instantiate(emptyDataPanelObj, DataPanelParent);
            }
        }

        userCount.text = BackendManager.Instance.userDataList.Count + "/" + 5;
        
        BackendManager.Instance.FindTeamMatchCard(BackendManager.Instance.userDataList.Count);
    }
    
    IEnumerator RandomTipTextCor()
    {
        int currentTipIndex = 0;
        
        while (true)
        {
            int randomVal = Random.Range(0, TipStrings.Count);

            if (currentTipIndex != randomVal)
            {
                TipText.text = TipStrings[randomVal];
                
                yield return new WaitForSeconds(3);
            }
        }
    }

    public void CreateRoom()
    {
        _backendManager.CreateMatchRoom();

        BackendManager.Instance.userDataList.Clear();

        foreach (Transform child in DataPanelParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        SelfDataInit();
    }

    public void FastMatchUIOpen()
    {
        UIManager.Instance.OpenPopup(UIManager.Instance.FastMatchingPopupPrefab);
    }

    public void InviteUIOpen()
    {
        UIManager.Instance.OpenPopup(UIManager.Instance.DoInvitePrefab);
    }

    public void LeaveMatchingRoom()
    {
        _backendManager.LeaveMatchMaking();
    }

    IEnumerator WaitInitDataCor()
    {
        yield return new WaitUntil(() => _backendManager.isInitialize);

        //플레이를 마친 유저라면 메인화면으로 다시 이동
        if (!_backendManager.isPlayedUser)
        {
            readyToPlay.gameObject.SetActive(true);

            loginCheckButton.interactable = true;
            
            DataManager.Instance.Initialize();
        }
        else
        {
            ChangeUI(1);

            _backendManager.ResetInGameData();
            
            ADManager.Instance.ShowAD();
        }
    }

    public void MatchingCancel()
    {
        Backend.Match.CancelMatchMaking();
        
        LeaveMatchingRoom();
    }

    IEnumerator RabbitBlinkEye()
    {
        while (true)
        {
            RabbitBlink(true);
            
            yield return new WaitForSeconds(2f);
            
            RabbitBlink(false);
            
            yield return new WaitForSeconds(0.2f);
            
            RabbitBlink(true);

            yield return new WaitForSeconds(1f);
        
            RabbitBlink(false);
        
            yield return new WaitForSeconds(0.2f);
        
            RabbitBlink(true);

            yield return new WaitForSeconds(0.3f);
        
            RabbitBlink(false);
        
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void RabbitBlink(bool isOn)
    {
        Rabbit1.GameObject().SetActive(isOn);
        Rabbit2.GameObject().SetActive(!isOn);
    }

    public void SelfDataInit()
    {
        roomNameText.text = BackendManager.Instance.userInfo.Nickname + "의 방";
        userCount.text = "1/5";
        btn_Invite.interactable = true;
        btn_MatchStart.interactable = true;
        
        GameObject getDataPanel = Instantiate(userInfoDataPanelObj, DataPanelParent);

        UI_UserIDPanel_PCI panelData = getDataPanel.GetComponent<UI_UserIDPanel_PCI>();
        
        panelData.txt_userName.text = BackendManager.Instance.userInfo.Nickname;

        UserData mydata = new UserData();

        mydata.playerName = BackendManager.Instance.userInfo.Nickname;
        mydata.playerToken = "";
        mydata.isSuperGamer = false;
        
        BackendManager.Instance.userDataList.Add(mydata);

        for (int i = 0; i < 4; i++)
        {
            Instantiate(emptyDataPanelObj, DataPanelParent);
        }
    }

    public void OpenCSWeb()
    {
// margin(빈 여백)이 10인 1대1 문의 창을 생성합니다.
#if UNITY_ANDROID
        isQuestionViewOpen =  BackEnd.Support.Android.Question.OpenQuestionView(questionAuthorize, myIndate, 10, 10, 10, 10);
                BackEnd.Support.Android.Question.OpenQuestionView(Backend.Question.GetQuestionAuthorize().ToString(), BackendManager.Instance.userInfo.UserIndate, 0, 0,0, 0);
        //BackEnd.Support.iOS.Question.OpenQuestionView(string questionAuthorize, string myIndate, int left, int top, int right, int bottom) -> bool
        
        BackendReturnObject bro = Backend.Question.GetQuestionAuthorize();
        string questionAuthorize = bro.GetReturnValuetoJSON()["authorize"].ToString();


        BackendReturnObject bro2 = Backend.BMember.GetUserInfo();
        string myIndate = bro2.GetReturnValuetoJSON()["row"]["inDate"].ToString();

        bool isQuestionViewOpen = false;
        
        if(isQuestionViewOpen)
        {
            Debug.Log("1대1 문의 창이 생성되었습니다");
        }
#endif
        UIManager.Instance.OpenRecyclePopup("시스템 메세지", "현재 윈도우 버전은 지원하지 않습니다.", null);
    }
    
}
