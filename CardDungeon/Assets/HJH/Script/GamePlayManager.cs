    using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
    public MainUI_HJH mainUi;
    public Player_HJH[] players;
    public PlayerDeck_HJH playerDeck;
    public int myIdx;
    public GameBoard_PCI gameBoard;
    // Start is called before the first frame update
    void Start()
    {
        //서버랑 소통하고 나서 로컬 플레이어의 인덱스를 받아왔다는 가정 하에 코드 작성

        mainUi.myPlayer = players[myIdx];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void CardGo(int playerIdx,int cardIdx) //카드 사용, 서버와 통신 해야됨
    {
        CardManager.Instance.OnCardStart(players[playerIdx].transform, cardIdx);
    }

    public void GameOver()
    {

    }

    public void GoDamage(Vector2Int pos,int damage)
    {
        for(int i =0; i<players.Length;i++)
        {
            Vector2Int vec = new Vector2Int((int)players[i].transform.position.x, (int)players[i].transform.position.y);
            if(vec == pos)
            {
                players[i].HP -= damage;
            }
        }
    }

}
