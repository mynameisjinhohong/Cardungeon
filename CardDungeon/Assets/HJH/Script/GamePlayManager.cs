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
    //
    public MainUI_HJH mainUi;
    public Player_HJH[] players;
    public List<GameObject> playerPrefabs;
    public PlayerDeck_HJH playerDeck;
    public int myIdx;
    public int SuperGamerIdx;
    public GameBoard_PCI gameBoard;
    public GameObject playerPool;
    public Transform[] PlayerSpawnPosition;

    #region 호스트
    public bool isHost;
    public Queue<Message> messageQueue;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitforGameStart());
    }

    IEnumerator WaitforGameStart()
    {
        yield return new WaitUntil(() => BackendManager.Instance.isLoadGame);
        
        InitializeGame();
        //서버랑 소통하고 나서 로컬 플레이어의 인덱스를 받아왔다는 가정 하에 코드 작성
    }
    
    public void InitializeGame()
    {
        if (isHost)
        {
            messageQueue= new Queue<Message>();
        }
        if (Backend.Match.OnMatchRelay == null)
        {
            Backend.Match.OnMatchRelay = (MatchRelayEventArgs args) => {
                if (args.From.NickName == BackendManager.Instance.Nickname)
                {
                    return;
                }
                var strByte = System.Text.Encoding.Default.GetString(args.BinaryUserData);
                Message msg = JsonUtility.FromJson<Message>(strByte);
               
                if (isHost)
                {
                    messageQueue.Enqueue(msg);
                }
                else
                {
                    if (args.From.NickName == "슈퍼방장")
                    {
                        CardRealGo(msg.playerIdx, msg.cardIdx);
                    }
                }
                //Debug.Log($"서버에서 받은 데이터 : {args.From.NickName} : {msg.ToString()}");
            };
        }
        //gameRecord = new Stack<SessionId>();
        //GameManager.OnGameOver += OnGameOver;
        //GameManager.OnGameResult += OnGameResult;
        //myPlayerIndex = SessionId.None;
        //SetPlayerAttribute();
        //OnGameStart();
        for (int i = 0; i < players.Length; i++)
        {
            if (i < BackendManager.Instance.UserDataList.Count)
            {
                GameObject PlayerPrefab = Instantiate(playerPrefabs[i], PlayerSpawnPosition[i]);

                Player_HJH playerHjh = PlayerPrefab.GetComponent<Player_HJH>();
            
                playerHjh.isSuperGamer = BackendManager.Instance.UserDataList[i].isSuperGamer;
                playerHjh.PlayerName   = BackendManager.Instance.UserDataList[i].playerName;
                playerHjh.PlayerToken  = BackendManager.Instance.UserDataList[i].playerToken;
            
                if (BackendManager.Instance.UserDataList[i].isSuperGamer)
                {
                    SuperGamerIdx = i;
                }
                
                players[i].gameObject.SetActive(true);
            }
            else
            {
                players[i].gameObject.SetActive(false);
            }

        }

        for (int i = 0; i < BackendManager.Instance.UserDataList.Count; i++)
        {
            if (BackendManager.Instance.Nickname == BackendManager.Instance.UserDataList[i].playerName)
            {
                myIdx = i;

                if (myIdx == SuperGamerIdx)
                    isHost = true;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isHost)
        {
            if (messageQueue.Count > 0)
            {
                Message m = messageQueue.Dequeue();
                SendData(m);
                CardRealGo(m.playerIdx, m.cardIdx);
            }
        }

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

    public void SendData(Message mes)
    {
        var jsonData = JsonUtility.ToJson(mes); // 클래스를 json으로 변환해주는 함수
        var dataByte = System.Text.Encoding.UTF8.GetBytes(jsonData); // json을 byte[]로 변환해주는 함수
        Backend.Match.SendDataToInGameRoom(dataByte);
    }


    public void CardGo(int playerIdx,int cardIdx) //카드 사용, 서버와 통신 해야됨
    {
        if (isHost)
        {
            Message mes = new Message();
            mes.playerIdx = playerIdx;
            mes.cardIdx = cardIdx;
            messageQueue.Enqueue(mes);
        }
        else
        {
            Message mes = new Message();
            mes.playerIdx = playerIdx;
            mes.cardIdx = cardIdx;
            SendData(mes);
        }

    }

    public void CardRealGo(int playerIdx,int cardIdx)
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
