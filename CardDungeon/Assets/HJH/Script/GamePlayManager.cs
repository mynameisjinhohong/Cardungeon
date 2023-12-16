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
        //������ �����ϰ� ���� ���� �÷��̾��� �ε����� �޾ƿԴٴ� ���� �Ͽ� �ڵ� �ۼ�
        for (int i =0; i<BackEndServerManager.GetInstance().nameList.Count; i++)
        {
            if (BackEndServerManager.GetInstance().nameList[i] == BackEndServerManager.GetInstance().myNickName)
            {
                myIdx = i;
            }
        } //�ε��� �޾ƿ���


        mainUi.myPlayer = players[myIdx];
    }
    public bool InitializeGame()
    {
        if (!playerPool)
        {
            Debug.Log("Player Pool Not Exist!");
            return false;
        }
        Debug.Log("���� �ʱ�ȭ ����");
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
                if(args.From.NickName == "���۹���")
                {

                }
                //Debug.Log($"�������� ���� ������ : {args.From.NickName} : {msg.ToString()}");
            };
        }

        Message message = new Message();
        var jsonData = JsonUtility.ToJson(message); // Ŭ������ json���� ��ȯ���ִ� �Լ�
        var dataByte = System.Text.Encoding.UTF8.GetBytes(jsonData); // json�� byte[]�� ��ȯ���ִ� �Լ�
        Backend.Match.SendDataToInGameRoom(dataByte);
    }


    public void CardGo(int playerIdx,int cardIdx) //ī�� ���, ������ ��� �ؾߵ�
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
