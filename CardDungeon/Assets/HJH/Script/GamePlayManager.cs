using System;
using BackEnd;
using BackEnd.Tcp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GamePlayManager : Singleton<GamePlayManager>
{
    //
    public MainUI_HJH mainUi;
    public List<Player_HJH> players;
    public List<GameObject> playerPrefabs;
    public PlayerDeck_HJH playerDeck;
    public int myIdx;
    public int SuperGamerIdx;
    public GameBoard_PCI gameBoard;
    public GameObject playerPool;
    public Transform[] PlayerSpawnPosition;
    public bool isDataCheck = false;
    #region ȣ��Ʈ
    public Queue<Message> messageQueue;
    #endregion
    public List<Color> colorList;

    public GameObject chaserObj;
    public Chaser chaser;
    
    // Start is called before the first frame update
    void Start()
    {
        //SetResolution();
        BackendManager.Instance.UserDataList.Clear();
        
        StartCoroutine(WaitforGameStart());
    }

    public void SetResolution()
    {
        int setWidth = 1920; // ����� ���� �ʺ�
        int setHeight = 1080; // ����� ���� ����

        int deviceWidth = Screen.width; // ��� �ʺ� ����
        int deviceHeight = Screen.height; // ��� ���� ����

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution �Լ� ����� ����ϱ�

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // ����� �ػ� �� �� ū ���
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // ���ο� �ʺ�
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
        }
        else // ������ �ػ� �� �� ū ���
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // ���ο� ����
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
        }
    }
    IEnumerator WaitforGameStart()
    {
        yield return new WaitUntil(() => BackendManager.Instance.isLoadGame);
        BackendManager.Instance.UserDataList.Sort((UserData lhs, UserData rhs) =>
        {
            if (int.Parse(lhs.playerToken) < int.Parse(rhs.playerToken))
            {
                return 1;
            }
            else if (lhs.playerToken == rhs.playerToken)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        });
        DataInit();

        yield return new WaitUntil(() => isDataCheck);

        InitializeGame();
        //������ �����ϰ� ���� ���� �÷��̾��� �ε����� �޾ƿԴٴ� ���� �Ͽ� �ڵ� �ۼ�
    }

    public void DataInit()
    {
        players = new List<Player_HJH>();
        for (int i = 0; i < playerPrefabs.Count; i++)
        {
            if (i < BackendManager.Instance.UserDataList.Count)
            {
                GameObject PlayerPrefab = Instantiate(playerPrefabs[i], PlayerSpawnPosition[i]);
                Player_HJH playerHjh = PlayerPrefab.GetComponent<Player_HJH>();
                players.Add(playerHjh);
                playerHjh.isSuperGamer = BackendManager.Instance.UserDataList[i].isSuperGamer;
                playerHjh.PlayerName.text = BackendManager.Instance.UserDataList[i].playerName;

                if (BackendManager.Instance.userInfo.Nickname == BackendManager.Instance.UserDataList[i].playerName)
                {
                    playerHjh.isMine = true;
                    mainUi.myPlayer = playerHjh;
                    mainUi.myPlayer.isMeIconObj.SetActive(true);
                    mainUi.myPlayer.KeysOnValueChanged += mainUi.SetKeysUI;
                }
                playerHjh.PlayerToken = BackendManager.Instance.UserDataList[i].playerToken;

                if (BackendManager.Instance.UserDataList[i].isSuperGamer)
                {
                    SuperGamerIdx = i;
                }

                players[i].gameObject.SetActive(true);
            }

        }

        for (int i = 0; i < BackendManager.Instance.UserDataList.Count; i++)
        {
            if (BackendManager.Instance.userInfo.Nickname == BackendManager.Instance.UserDataList[i].playerName)
            {
                myIdx = i;

                Transform parentTransform = PlayerSpawnPosition[myIdx].transform;

                Camera.main.transform.SetParent(parentTransform.GetChild(0));

                Camera.main.transform.localPosition = new Vector3(0.5f, 0.5f, -10f);
            }
        }

        mainUi.playerBG.color = colorList[myIdx];
        
        isDataCheck = true;
    }
    public bool CardIdxCheckNoPlayer(int cardIdx, Transform playerPos)
    {
        Vector2 goPos = new Vector2();
        switch (Mathf.Abs(cardIdx))
        {
            case 1:
                if(cardIdx > 0)
                {
                    goPos = new Vector2(playerPos.position.x, playerPos.position.y + 2);
                }
                else
                {
                    goPos = new Vector2(playerPos.position.x, playerPos.position.y + 1);
                }
                break;
            case 2:
                if (cardIdx > 0)
                {
                    goPos = new Vector2(playerPos.position.x + 2, playerPos.position.y);
                }
                else
                {
                    goPos = new Vector2(playerPos.position.x + 1, playerPos.position.y);
                }
                break;
            case 3:
                if (cardIdx > 0)
                {
                    goPos = new Vector2(playerPos.position.x - 2, playerPos.position.y);
                }
                else
                {
                    goPos = new Vector2(playerPos.position.x - 1, playerPos.position.y);
                }
                break;
            case 4:
                if (cardIdx > 0)
                {
                    goPos = new Vector2(playerPos.position.x, playerPos.position.y - 2);
                }
                else
                {
                    goPos = new Vector2(playerPos.position.x, playerPos.position.y - 1);
                }
                break;
            case 5:
                if (cardIdx > 0)
                {
                    goPos = new Vector2(playerPos.position.x, playerPos.position.y - 2);
                }
                else
                {
                    goPos = new Vector2(playerPos.position.x, playerPos.position.y - 1);
                }
                break;
        }
        if(Mathf.Abs(cardIdx) == 6)
        {
            if(cardIdx > 0)
            {
                Random.InitState(CardManager.Instance.seed);
                int x = Random.Range(-2, 3);
                int y = Random.Range(-2, 3);
                if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)playerPos.transform.position.x + x, (int)playerPos.transform.position.y + y)))
                {
                    goPos = new Vector3(playerPos.transform.position.x + x, playerPos.transform.position.y + y, playerPos.transform.position.z);
                }
                else
                {
                    for (int i = -2; i < 3; i++)
                    {
                        for (int j = -2; j < 3; j++)
                        {
                            if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)playerPos.transform.position.x + j, (int)playerPos.transform.position.y + i)))
                            {
                                goPos = new Vector3(playerPos.transform.position.x + j, playerPos.transform.position.y + i, playerPos.transform.position.z);
                            }
                        }
                    }
                }
            }
            else
            {
                Random.InitState(CardManager.Instance.seed);
                int x = Random.Range(-3, 4);
                int y = Random.Range(-3, 4);
                if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)playerPos.transform.position.x + x, (int)playerPos.transform.position.y + y)))
                {
                    goPos = new Vector3(playerPos.transform.position.x + x, playerPos.transform.position.y + y, playerPos.transform.position.z);
                }
                else
                {
                    for (int i = -3; i < 4; i++)
                    {
                        for (int j = -3; j < 4; j++)
                        {
                            if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)playerPos.transform.position.x + j, (int)playerPos.transform.position.y + i)))
                            {
                                goPos = new Vector3(playerPos.transform.position.x + j, playerPos.transform.position.y + i, playerPos.transform.position.z);
                            }
                        }
                    }
                }
            }
        }
        return CheckNoPlayer(goPos);


    }


    public bool CheckNoPlayer(Vector2 goPos)
    {
        for (int i = 0; i < players.Count; i++)
        {
            Vector2 vec = new Vector2((int)players[i].transform.position.x, (int)players[i].transform.position.y);
            if ((Vector2)goPos == vec)
            {
                return false;
            }
        }
        return true;
    }

    public void InitializeGame()
    {
        if (BackendManager.Instance.isMeSuperGamer)
        {
            messageQueue = new Queue<Message>();
        }
        if (Backend.Match.OnMatchRelay == null)
        {
            Backend.Match.OnMatchRelay = (MatchRelayEventArgs args) =>
            {
                if (args.From.NickName == BackendManager.Instance.userInfo.Nickname)
                {
                    return;
                }
                var strByte = System.Text.Encoding.Default.GetString(args.BinaryUserData);
                Message msg = JsonUtility.FromJson<Message>(strByte);

                if (BackendManager.Instance.isMeSuperGamer)
                {
                    messageQueue.Enqueue(msg);
                }
                else
                {
                    if (args.From.NickName == BackendManager.Instance.UserDataList[SuperGamerIdx].playerName)
                    {
                        if (msg.playerIdx == -10)
                        {
                            int head = BackendManager.Instance.UserDataList.Count;
                            switch (head)
                            {
                                case 0:
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                    gameBoard.Generate(msg.cardIdx, 14, 14);
                                    break;
                                case 6:
                                    gameBoard.Generate(msg.cardIdx, 20, 20);
                                    break;
                                case 7:
                                case 8:
                                default:
                                    gameBoard.Generate(msg.cardIdx, 30, 30);
                                    break;
                            }
                            CardManager.Instance.seed = msg.cardIdx;
                        }
                        else
                        {
                            Debug.Log(msg.playerIdx + "  " + msg.cardIdx);
                            CardRealGo(msg.playerIdx, msg.cardIdx);
                        }
                    }
                }
                //Debug.Log($"�������� ���� ������ : {args.From.NickName} : {msg.ToString()}");
            };
        }
        if (BackendManager.Instance.isMeSuperGamer)
        {
            Message m = new Message();
            m.playerIdx = -10;
            m.cardIdx = Random.Range(0, 100);
            SendData(m);
            CardManager.Instance.seed = m.cardIdx;
            int head = BackendManager.Instance.UserDataList.Count;
            switch (head)
            {
                case 0:
                case 1:
                case 2:
                    gameBoard.Generate(m.cardIdx, 20, 20);
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                    gameBoard.Generate(m.cardIdx, 30, 30);
                    break;
                case 7:
                case 8:
                default:
                    gameBoard.Generate(m.cardIdx, 40, 40);
                    break;
            }
        }

        //gameRecord = new Stack<SessionId>();
        //GameManager.OnGameOver += OnGameOver;
        //GameManager.OnGameResult += OnGameResult;
        //myPlayerIndex = SessionId.None;
        //SetPlayerAttribute();
        //OnGameStart();
        
        Backend.Match.OnSessionOffline = (MatchInGameSessionEventArgs args) => {
            
            Debug.Log(args.GameRecord.m_nickname + "���� ������ �����ϼ̽��ϴ�.");

            int OutUserIndex = 0;
            
            for (int i = 0; i < BackendManager.Instance.UserDataList.Count; i++)
            {
                if (BackendManager.Instance.UserDataList[i].playerName == args.GameRecord.m_nickname)
                {
                    OutUserIndex = i;
                }
            }

            BackendManager.Instance.UserDataList.RemoveAt(OutUserIndex);
            
            Destroy(PlayerSpawnPosition[OutUserIndex].gameObject);

            if (BackendManager.Instance.UserDataList.Count <= 1)
            {
                Debug.Log("�¸�ó��");
                GameWin(false);
            }
        };
        
        Backend.Match.OnChangeSuperGamer = (MatchInGameChangeSuperGamerEventArgs args) => {
            Debug.Log("���۰��̸Ӱ� ������ �����Ͽ� ���۰��̸Ӹ� �缱���մϴ�");
            
            for (int i = 0; i < BackendManager.Instance.UserDataList.Count; i++)
            {
                if (BackendManager.Instance.UserDataList[i].playerName == args.NewSuperUserRecord.m_nickname)
                {
                    Debug.Log("���ο� ���۰��̸Ӵ�" + BackendManager.Instance.UserDataList[i].playerName + "�� �Դϴ�");

                    BackendManager.Instance.UserDataList[i].isSuperGamer = true;
                    
                    BackendManager.Instance.isMeSuperGamer =
                        BackendManager.Instance.UserDataList[i].playerName == BackendManager.Instance.userInfo.Nickname;
                }
            }
        };
    }
    // Update is called once per frame
    void Update()
    {
        try
        {
            if (!isDataCheck)
            {
                Debug.LogWarning("isDataCheck is false, skipping Update.");
                return;
            }

            if (BackendManager.Instance == null)
            {
                Debug.LogError("BackendManager.Instance is null.");
                return;
            }

            if (BackendManager.Instance.isMeSuperGamer)
            {
                if (messageQueue == null)
                {
                    Debug.LogError("messageQueue is null.");
                    return;
                }

                if (messageQueue.Count > 0)
                {
                    Message m = messageQueue.Dequeue();
                    if (m == null)
                    {
                        Debug.LogError("Dequeued message is null.");
                        return;
                    }

                    SendData(m);
                    Debug.Log($"Processing message for playerIdx: {m.playerIdx}, cardIdx: {m.cardIdx}");
                    CardRealGo(m.playerIdx, m.cardIdx);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Exception in Update: {e}");
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
        try
        {
            Backend.Match.SendDataToInGameRoom(dataByte);
        }
        catch
        {
            Debug.Log("���� ������");
        }
    }


    public void CardGo(int playerIdx, int cardIdx) //ī�� ���, ������ ��� �ؾߵ�
    {
        if (BackendManager.Instance.isMeSuperGamer)
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

    public void CardRealGo(int playerIdx, int cardIdx)
    {
        if (playerIdx == 9)
        {
            CardManager.Instance.OnCardStart(chaser.transform, cardIdx);
        }
        else
        {
            CardManager.Instance.OnCardStart(players[playerIdx].transform, cardIdx);
        }
    }

    public void GameOver()
    {
        Camera.main.transform.SetParent(null);
        players[myIdx].gameObject.SetActive(false);
        mainUi.GameOver();
    }


    public void GoDamage(Vector2Int pos, int damage)
    {
        for (int i = 0; i < players.Count; i++)
        {
            Vector2Int vec = new Vector2Int((int)players[i].transform.position.x, (int)players[i].transform.position.y);
            if (vec == pos)
            {
                if (players[i].HP > 0)
                {
                    players[i].HP -= damage;
                }
            }
        }
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].isMine) continue;
            if (players[i].HP > 0)
            {
                return;
            }
        }
        GameWin(false);
    }

    public void GameWin(bool isEscape)
    {
        mainUi.winImage.SetActive(true);
        if (mainUi.winImage.TryGetComponent<Button>(out Button btn))
        {
            btn.onClick.AddListener(GoTitle);
        }

        string allKillWin = "�¸��Ͽ����ϴ�!";

        string esacpeWin = "Ż�� ����!";

        mainUi.winTypeText.text = isEscape ? esacpeWin : allKillWin;
    }

    public void GoTitle()
    {
        mainUi.winImage.SetActive(false);
        if (mainUi.winImage.TryGetComponent<Button>(out Button btn))
        {
            btn.onClick.RemoveListener(GoTitle);
        }
    }
    
    //������ �ڵ� ���ܵ�
    // Backend.Match.IsGameRoomActivate( callback =>
    // {
    //     switch (callback.GetStatusCode())
    //     {
    //         case "200" :
    //             Debug.Log("�濡 �������� �����մϴ�.");
    //             var roomInfo = callback.GetReturnValuetoJSON();
    //             var addr = roomInfo["serverPublicHostName"].ToString();
    //             var port = Convert.ToUInt16(roomInfo["serverPort"].ToString());
    //             ErrorInfo errorInfo = null;
    //             
    //             if(Backend.Match.JoinGameServer(addr, port, true, out errorInfo) == false)
    //             {
    //                 // ���� Ȯ��
    //                 Debug.Log("������ �õ��մϴ�.");
    //                 return;
    //             }
    //             break;
    //         case "404" :
    //             Debug.Log("���� ������ ������ ���� �����ϴ�.");
    //             UIManager.Instance.OpenRecyclePopup("�ȳ�", "�������� �Ұ����մϴ� Ÿ��Ʋ�� �̵��մϴ�.", GoTitle);
    //             break;
    //     }
    // });
}
