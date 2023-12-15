    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
    public Player_HJH[] players;
    public int myIdx;
    public GameBoard_PCI gameBoard;
    // Start is called before the first frame update
    void Start()
    {
        
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

}
