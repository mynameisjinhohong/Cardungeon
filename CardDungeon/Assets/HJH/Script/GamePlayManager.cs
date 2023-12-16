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

    #region ȣ��Ʈ
    public bool isHost;
    public Queue<Message> messageQueue;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        InitializeGame();
        //������ �����ϰ� ���� ���� �÷��̾��� �ε����� �޾ƿԴٴ� ���� �Ͽ� �ڵ� �ۼ�

        mainUi.myPlayer = players[myIdx];
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
                var strByte = System.Text.Encoding.Default.GetString(args.BinaryUserData);
                Message msg = JsonUtility.FromJson<Message>(strByte);
                if (isHost)
                {
                    if(!args.From.IsRemote)
                    {
                        messageQueue.Enqueue(msg);
                    }
                }
                else
                {
                    if (args.From.NickName == "���۹���" && !args.From.IsRemote)
                    {
                        CardRealGo(msg.playerIdx, msg.cardIdx);
                    }
                }
                //Debug.Log($"�������� ���� ������ : {args.From.NickName} : {msg.ToString()}");
            };
        }
        //gameRecord = new Stack<SessionId>();
        //GameManager.OnGameOver += OnGameOver;
        //GameManager.OnGameResult += OnGameResult;
        //myPlayerIndex = SessionId.None;
        //SetPlayerAttribute();
        //OnGameStart();
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
        var jsonData = JsonUtility.ToJson(mes); // Ŭ������ json���� ��ȯ���ִ� �Լ�
        var dataByte = System.Text.Encoding.UTF8.GetBytes(jsonData); // json�� byte[]�� ��ȯ���ִ� �Լ�
        Backend.Match.SendDataToInGameRoom(dataByte);
    }


    public void CardGo(int playerIdx,int cardIdx) //ī�� ���, ������ ��� �ؾߵ�
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
