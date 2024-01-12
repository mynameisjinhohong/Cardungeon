using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    private Canvas canvas;

    public int SceneNumber = 0;

    public List<GameObject> PopupList;

    public GameObject CurrentPopup;

    public CurrentUIStatus CurrentUIStatus; 
    
    private Transform PopupListParent;
    
    private Action ABBAction;

    public GameObject AccountLoginPrefab;
    public GameObject AccountSignUpPrefab;
    public GameObject FindAccountPrefab;
    
    public GameObject GoogleLoginPrefab;

    public GameObject GuestLoginPrefab;

    public GameObject NickNamePrefab;

    public GameObject RecyclePopupPrefab;

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

        PopupListParent = canvas.transform.GetChild(canvas.transform.childCount - 1).transform;
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
}
