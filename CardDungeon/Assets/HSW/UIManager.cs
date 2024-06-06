using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd.Tcp;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    public GameObject GoogleLoginPrefab;

    public GameObject GuestLoginPrefab;

    public GameObject NickNamePrefab;

    public GameObject RecyclePopupPrefab;

    public GameObject GetInvitePopupPrefab;

    public GameObject SettingPopupPrefab;

    public GameObject TutorialPopupPrefab;
    
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
            PopupListPop();
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

        if (target.action == null)
            target.action = PopupListPop;
        else
            target.action = action;

        PopupListAddNoneABB(Popup);
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
}
