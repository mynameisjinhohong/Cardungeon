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
            BackendManager.Instance.GuestLoginSequense();
            //팝업매니저 닉네임 입력 팝업   
        }
        else
        {
            if (!BackendManager.Instance.isInitialize)
                BackendManager.Instance.StartTokenLogin();
            
            ChangeUI(1);
            UserNickName.text = BackendManager.Instance.Nickname;

            MatchSetting();
        }
    }

    public void ChangeUI(int index)
    {
        for (int i = 0; i < UIList.Count; i++)
        {
            UIList[i].SetActive(i == index);
        }
    }

    public void MatchStartCor()
    {
        StartCoroutine(MatchSetting());
    }
    
    IEnumerator MatchSetting()
    {
        matchingTest.GetMatchList();

        yield return new WaitForSeconds(0.5f);
        
        matchingTest.CreateMatchRoom();

        yield return new WaitForSeconds(0.5f);

        matchingTest.JoinMatchMakingServer();

        yield return new WaitForSeconds(0.5f);
        
        matchingTest.RequestMatchMaking();
    }

    public void MatchCancel()
    {
        Backend.Match.LeaveMatchMakingServer();
        ChangeUI(1);
    }
    

}
