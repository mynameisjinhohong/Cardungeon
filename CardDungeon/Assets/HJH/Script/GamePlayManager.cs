    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
    public GameObject[] players;
    public int myIdx;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void CardGo(int cardIdx) //ī�� ���, ������ ��� �ؾߵ�
    {
        CardManager.Instance.OnCardStart(players[myIdx].transform, cardIdx);
    }

}
