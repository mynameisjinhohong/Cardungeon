using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchController : MonoBehaviour
{
    public List<GameObject> UIList;

    public void CheckPlayersLoginWay()
    {
        if(BackendManager.Instance.checkLoginWayData == -1)
        {
            BackendManager.Instance.GuestLoginSequense();
            //�˾��Ŵ��� �г��� �Է� �˾�   
        }
        else
        {
            BackendManager.Instance.StartTokenLogin();
            ChangeUI(1);
        }
    }

    public void ChangeUI(int index)
    {
        for (int i = 0; i < UIList.Count; i++)
        {
            UIList[i].SetActive(i == index);
        }
    }
    

}
