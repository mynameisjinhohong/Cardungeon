using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupManager : Singleton<PopupManager>
{
    public Canvas canvas;

    public int SceneNumber = 0;

    public List<GameObject> PopupList;

    private GameObject CurrentPopup;
    
    private GameObject PopupListParent;
    
    private Action ABBAction;
    
    public SignUpPopup SignUpPrefab;

    //public RecyclePopup RecyclePopup;

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
    
    public void SignUpPopup(GameObject prefab)
    {
        PopupListAddNoneABB(prefab);
    }
    
    public void FindCanvas()
    {
        canvas = null;
        if (SceneManager.GetActiveScene().name == "0_MatchScene")
        {
            SceneNumber = 2;
            canvas = GameObject.Find("MatchCanvas").GetComponent<Canvas>();
            PopupListParent = canvas.gameObject;
        }
        // if (SceneManager.GetActiveScene().name == "MainScene")
        // {
        //     GameObject PopupCanvasFind = GameObject.Find("PopupCanvas");
        //
        //     SceneNumber = 1;
        //     canvas = PopupCanvasFind.GetComponent<Canvas>();
        //     PopupListParent = PopupCanvasFind;
        // }
        //
        // if (SceneManager.GetActiveScene().name == "FirstLoadScene")
        // {
        //     SceneNumber = 0;
        //     canvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        //
        //     PopupListParent = canvas.gameObject;
        // }
            
    }
    
    public void PopupListAddABB(GameObject gameObject, Action action)
    {
        PopupList.Add(gameObject);

        gameObject.SetActive(true);
        
        CurrentPopup = gameObject;

        ABBAction = action;
    }
    
    public void PopupListAddNoneABB(GameObject gameObject)
    {
        PopupList.Add(gameObject);

        gameObject.SetActive(true);
        
        CurrentPopup = gameObject;
    }
}
