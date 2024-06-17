using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackEnd.Tcp;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private static UIManager instance;
    
    private Canvas canvas;

    public int SceneNumber = 0;

    public List<GameObject> PopupList;

    public GameObject CurrentPopup;

    public CurrentUIStatus CurrentUIStatus; 
    
    public Transform PopupListParent;
    
    private Action ABBAction;

    public GameObject AccountLoginPrefab;
    public GameObject AccountSignUpPrefab;
    public GameObject FindIDPrefab;
    public GameObject ResetPWPrefab;
    
    public GameObject DoInvitePrefab;

    public GameObject GuestLoginPrefab;

    public GameObject NickNamePrefab;

    public GameObject RecyclePopupPrefab;

    public GameObject GetInvitePopupPrefab;

    public GameObject SettingPopupPrefab;

    public GameObject TutorialPopupPrefab;

    public GameObject FastMatchingPopupPrefab;

    public GameObject IndicatorPopupPrefab;

    private Coroutine indicatorCoroutine;

    public Sprite indicatorImage1;
    public Sprite indicatorImage2;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        // 모든 씬에서 유지
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindCanvas();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentPopup.GetComponent<FastMatchUI>())
            {
                CurrentPopup.GetComponent<FastMatchUI>().CloseBtnAction();
            }
            else
            {
                PopupListPop();
            }
        }
    }

    public void FindCanvas()
    {
        canvas = null;
        
        canvas = FindObjectOfType<Canvas>();
        
        Transform popupParent = canvas.transform.Find("PopupTransform");

        if (popupParent != null)
        {
            PopupListParent = popupParent;
        }
        else
        {
            Debug.LogError("팝업 생성 위치 탐색 실패 오브젝트 이름이 PopupTransform 인지 확인해주세요");
        }
    }

    public void PopupListAddABB(GameObject gameObject, Action action)
    {
        PopupList.Add(gameObject);

        gameObject.SetActive(true);
        
        CurrentPopup = gameObject;

        ABBAction = action;

        CurrentUIStatus = CurrentUIStatus.ABBPopup;
        
        DarkBGCheck();
    }
    
    public void PopupListAddNoneABB(GameObject gameObject)
    {
        PopupList.Add(gameObject);

        gameObject.SetActive(true);
        
        CurrentPopup = gameObject;

        DarkBGCheck();
    }

    public void PopupListPop()
    {
        if (PopupList != null && PopupList.Count > 0)
        {
            if (CurrentPopup != null && CurrentPopup.activeSelf)
            {
                Destroy(CurrentPopup);
                    
                PopupList.RemoveAt(PopupList.Count - 1);

                if (PopupList.Count > 0)
                {
                    CurrentPopup = PopupList[0];
                }
            }
        }

        DarkBGCheck();
    }

    public void AllPopupClear()
    {
        foreach (GameObject popup in PopupList)
        {
            if (popup != null && popup.activeSelf)
            {
                Destroy(popup);
            }
        }
        PopupList.Clear();
        CurrentPopup = null;

        DarkBGCheck();
    }

    private void DarkBGCheck()
    {
        PopupListParent.gameObject.SetActive(PopupList.Count > 0);
    }
    
    public void OpenPopup(GameObject PopupObj)
    {
        GameObject Popup = Instantiate(PopupObj, PopupListParent);
        
        PopupListAddABB(Popup, PopupListPop);
    }

    public void OpenRecyclePopup(String title, String descript, Action action)
    {
        GameObject Popup = Instantiate(RecyclePopupPrefab, PopupListParent);

        RecyclePopup target = Popup.GetComponent<RecyclePopup>();

        target.title.text    = title;
        target.descript.text = descript;

        target.action = (action != null) ? action : PopupListPop;

        PopupListAddABB(Popup, action);
    }

    public void OpenInvitePopup(String title, SessionId sessionID, string roomToken)
    {
        GameObject Popup = Instantiate(GetInvitePopupPrefab, PopupListParent);
        
        InvitePopup target = Popup.GetComponent<InvitePopup>();

        target.titleText.text = title + "님이 초대하셨습니다.\n초대를 수락하시면 매칭룸으로 이동합니다.";
        target.InvitedRoomID = sessionID;
        target.InvitedRoomToken = roomToken;

        PopupListAddABB(Popup, PopupListPop);
    }

    public void OpenAnimationUI(GameObject animatedUI)
    {
        GameObject targetUIObj = Instantiate(animatedUI, PopupListParent);
        
        PopupListAddABB(targetUIObj, PopupListPop);
        
        FastMatchUI targetUI = targetUIObj.GetComponent<FastMatchUI>();

        targetUI.DoPanelAnim(true);
    }

    public void OpenIndicator()
    {
        if (indicatorCoroutine == null)
        {
            indicatorCoroutine = StartCoroutine(WaitSuperGamerSetCor());
        }
        else
        {
            StopCoroutine(indicatorCoroutine);
            
            indicatorCoroutine = StartCoroutine(WaitSuperGamerSetCor());
        }
    }

    IEnumerator WaitSuperGamerSetCor()
    {
        GameObject indicator = Instantiate(IndicatorPopupPrefab, PopupListParent);
        
        Image indicatorImg = indicator.GetComponent<Image>();
        
        bool isSuperGamerSetted = false;

        while (true)
        {
            isSuperGamerSetted = TryFindSuperGamer();

            indicatorImg.sprite = indicatorImage1;
            
            yield return new WaitForSeconds(1);

            indicatorImg.sprite = indicatorImage2;
            
            if (isSuperGamerSetted)
            {
                if (BackendManager.Instance.isMeSuperGamer)
                    GamePlayManager.Instance.newSuperGamerMessageQueueInit();
                
                yield return new WaitForSeconds(1);
                
                Destroy(indicator);
                break;
            }
        }
    }

    bool TryFindSuperGamer()
    {
        for (int i = 0; i < BackendManager.Instance.userDataList.Count; i++)
        {
            if (BackendManager.Instance.userDataList[i].isSuperGamer)
            {
                return true;
            }
        }

        return false;
    }
}
