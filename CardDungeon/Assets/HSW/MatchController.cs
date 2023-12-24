using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MatchController : MonoBehaviour
{
    public List<GameObject> UIList;

    public TextMeshProUGUI UserNickName;

    public GameObject LoginPopup;
    
    private MatchingTest matchingTest;

    public TextMeshProUGUI TipText;

    public int UIIndex = 0;
    
    public GameObject Rabbit1;
    public GameObject Rabbit2;

    public List<String> TipStrings;
    
    public void Start()
    {
        matchingTest = BackendManager.Instance.GetComponent<MatchingTest>();
        
        TipStrings.Add("뒤끝 서버가\n토끼들의 성장을 돕고있어요");
        TipStrings.Add("잠시만 기다려 주세요\n토끼들을 모아 훈련하는 중입니다.");
        TipStrings.Add("던전은 매번\n랜덤으로 생성됩니다.");
        TipStrings.Add("열쇠는 나무 상자나\n보유 중인 유저에게 얻을 수 있어요.");
        TipStrings.Add("열쇠를 얻었다면\n던전의 중심으로 달려가세요!");
        TipStrings.Add("기본 이동 카드는\nMP 소모가 없습니다.\n얼마든지 깡총깡총!");
        TipStrings.Add("맵 곳곳에 새로운 카드들이\n흩어져 있습니다.\n먼저 획득해서 강해지세요!");
        TipStrings.Add("카드에 쓰이는 MP는\n일정 시간이 지나면 충전됩니다.");
        TipStrings.Add("공격과 스킬 카드에 쓰이는 MP는 일정 시간이 지나면 충전됩니다.");
        TipStrings.Add("'카드버리기'가 노란색이\n아닐때는 MP가 소모됩니다.");
        TipStrings.Add("나무 상자에는\n행운 혹은 불행이 들어 있어요.\n운을 시험해 보세요!");
        TipStrings.Add("다른 유저가 먼저 탈출 하거나\nHP를 전부 잃으면 게임오버 됩니다.");
    }

    public void CheckPlayersLoginWay()
    {
        if(BackendManager.Instance.checkLoginWayData == -1 || BackendManager.Instance.Nickname == "")
        {
            LoginPopup.SetActive(true);

            BackendManager.Instance.GuestLoginSequense();
        }
        else
        {
            Debug.Log("자동로그인 실행 테스트");
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

        UIIndex = index;

        if (index == 1)
        {
            StartCoroutine(RabbitBlinkEye());
            Debug.Log("깜빡");
        }

        if (index == 2)
        {
            int randomVal = Random.Range(0, TipStrings.Count);
            TipText.text = TipStrings[randomVal];
            Debug.Log("텍스트");
        }
    }

    public void MatchStart()
    {
        matchingTest.GetMatchList();

        BackendManager.Instance.matchIndex = 6;
        
        matchingTest.JoinMatchMakingServer();
    }

    public void MatchCancel()
    {
        Backend.Match.CancelMatchMaking();
        ChangeUI(1);
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

}
