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

    private GameObject CurrentPopup;
    
    private Transform PopupListParent;
    
    private Action ABBAction;

    public GameObject AccountLoginPrefab;
    public GameObject AccountSignUpPrefab;
    public GameObject FindAccountPrefab;
    
    public GameObject GoogleLoginPrefab;

    public GameObject GuestLoginPrefab;

    public GameObject RecyclePopupPrefab;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindCanvas();
    }
    // 이벤트 리스너 등록
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 이벤트 리스너 제거
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
        target.action = action;
        
        PopupListAddNoneABB(Popup);
    }
}
