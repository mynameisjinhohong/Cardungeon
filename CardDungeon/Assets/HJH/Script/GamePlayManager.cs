using BackEnd.Tcp;
using BackEnd;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class GamePlayManager : Singleton<GamePlayManager>
{
    public MainUI_HJH mainUi;
    public Player_HJH[] players;
    public PlayerDeck_HJH playerDeck;
    public int myIdx;
    public GameBoard_PCI gameBoard;
    public GameObject playerPool;
    public GameObject playerPrefeb;
    // Start is called before the first frame update
    void Start()
    {
        InitializeGame();
        var matchInstance = BackEndMatchManager.GetInstance();
        if (matchInstance == null)
        {
            return;
        }
        if (matchInstance.isReconnectProcess)
        {
            InGameUiManager.GetInstance().SetStartCount(0, false);
            InGameUiManager.GetInstance().SetReconnectBoard(BackEndServerManager.GetInstance().myNickName);
        }
        //서버랑 소통하고 나서 로컬 플레이어의 인덱스를 받아왔다는 가정 하에 코드 작성
        for (int i =0; i<BackEndServerManager.GetInstance().nameList.Count; i++)
        {
            if (BackEndServerManager.GetInstance().nameList[i] == BackEndServerManager.GetInstance().myNickName)
            {
                myIdx = i;
            }
        } //인덱스 받아오기


        mainUi.myPlayer = players[myIdx];
    }
    public bool InitializeGame()
    {
        if (!playerPool)
        {
            Debug.Log("Player Pool Not Exist!");
            return false;
        }
        Debug.Log("게임 초기화 진행");
        //gameRecord = new Stack<SessionId>();
        //GameManager.OnGameOver += OnGameOver;
        //GameManager.OnGameResult += OnGameResult;
        //myPlayerIndex = SessionId.None;
        //SetPlayerAttribute();
        //OnGameStart();
        return true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    //public void CardData(int cardIdx,int playerIdx )
    //{
    //    CardMessage msg;
    //    msg = new CardMessage(cardIdx, playerIdx);
    //    if (BackEndMatchManager.GetInstance().IsHost())
    //    {
    //        BackEndMatchManager.GetInstance().AddMsgToLocalQueue(msg);
    //    }
    //    else
    //    {
    //        BackEndMatchManager.GetInstance().SendDataToInGame<KeyMessage>(msg);
    //    }
    //}

    public class Message
    {
        public int playerIdx;
        public int cardIdx;
    }

    public void SendData()
    {
        if (Backend.Match.OnMatchRelay == null)
        {
            Backend.Match.OnMatchRelay = (MatchRelayEventArgs args) => {
                var strByte = System.Text.Encoding.Default.GetString(args.BinaryUserData);
                Message msg = JsonUtility.FromJson<Message>(strByte);
                if(args.From.NickName == "슈퍼방장")
                {

                }
                //Debug.Log($"서버에서 받은 데이터 : {args.From.NickName} : {msg.ToString()}");
            };
        }

        Message message = new Message();
        var jsonData = JsonUtility.ToJson(message); // 클래스를 json으로 변환해주는 함수
        var dataByte = System.Text.Encoding.UTF8.GetBytes(jsonData); // json을 byte[]로 변환해주는 함수
        Backend.Match.SendDataToInGameRoom(dataByte);
    }


    public void CardGo(int playerIdx,int cardIdx) //카드 사용, 서버와 통신 해야됨
    {
        CardManager.Instance.OnCardStart(players[playerIdx].transform, cardIdx);
    }

    public void CardRealGo(int playerIdx,int cardIdx)
    {

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
