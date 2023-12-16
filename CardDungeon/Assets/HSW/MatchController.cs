using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchController : MonoBehaviour
{
    public List<GameObject> UIList;

    public TextMeshProUGUI UserNickName;

    private MatchingTest matchingTest;

    public void Start()
    {
        matchingTest = GetComponent<MatchingTest>();
    }

    public void CheckPlayersLoginWay()
    {
        if(BackendManager.Instance.checkLoginWayData == -1)
        {
            BackendManager.Instance.LoginPopup.SetActive(true);

            BackendManager.Instance.GuestLoginSequense();
        }
        else
        {
            if (!BackendManager.Instance.isInitialize)
                BackendManager.Instance.StartTokenLogin();
            
            ChangeUI(1);
            UserNickName.text = BackendManager.Instance.Nickname;

            MatchStart();
        }
    }

    public void ChangeUI(int index)
    {
        for (int i = 0; i < UIList.Count; i++)
        {
            UIList[i].SetActive(i == index);
        }
    }

    public void MatchStart()
    {
        matchingTest.GetMatchList();

        matchingTest.JoinMatchMakingServer();
    }

    public void MatchCancel()
    {
        Backend.Match.CancelMatchMaking();
        ChangeUI(1);
    }
    

}
