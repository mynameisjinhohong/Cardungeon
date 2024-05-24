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
    #region 호스트
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
        int setWidth = 1920; // 사용자 설정 너비
        int setHeight = 1080; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
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
        //서버랑 소통하고 나서 로컬 플레이어의 인덱스를 받아왔다는 가정 하에 코드 작성
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
                //Debug.Log($"서버에서 받은 데이터 : {args.From.NickName} : {msg.ToString()}");
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
            
            Debug.Log(args.GameRecord.m_nickname + "님이 연결을 종료하셨습니다.");

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
                Debug.Log("승리처리");
                GameWin(false);
            }
        };
        
        Backend.Match.OnChangeSuperGamer = (MatchInGameChangeSuperGamerEventArgs args) => {
            Debug.Log("슈퍼게이머가 접속을 종료하여 슈퍼게이머를 재선정합니다");
            
            for (int i = 0; i < BackendManager.Instance.UserDataList.Count; i++)
            {
                if (BackendManager.Instance.UserDataList[i].playerName == args.NewSuperUserRecord.m_nickname)
                {
                    Debug.Log("새로운 슈퍼게이머는" + BackendManager.Instance.UserDataList[i].playerName + "님 입니다");

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
        var jsonData = JsonUtility.ToJson(mes); // 클래스를 json으로 변환해주는 함수
        var dataByte = System.Text.Encoding.UTF8.GetBytes(jsonData); // json을 byte[]로 변환해주는 함수
        try
        {
            Backend.Match.SendDataToInGameRoom(dataByte);
        }
        catch
        {
            Debug.Log("연결 끊어짐");
        }
    }


    public void CardGo(int playerIdx, int cardIdx) //카드 사용, 서버와 통신 해야됨
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

        string allKillWin = "승리하였습니다!";

        string esacpeWin = "탈출 성공!";

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
    
    //재접속 코드 남겨둠
    // Backend.Match.IsGameRoomActivate( callback =>
    // {
    //     switch (callback.GetStatusCode())
    //     {
    //         case "200" :
    //             Debug.Log("방에 재접속이 가능합니다.");
    //             var roomInfo = callback.GetReturnValuetoJSON();
    //             var addr = roomInfo["serverPublicHostName"].ToString();
    //             var port = Convert.ToUInt16(roomInfo["serverPort"].ToString());
    //             ErrorInfo errorInfo = null;
    //             
    //             if(Backend.Match.JoinGameServer(addr, port, true, out errorInfo) == false)
    //             {
    //                 // 에러 확인
    //                 Debug.Log("재접속 시도합니다.");
    //                 return;
    //             }
    //             break;
    //         case "404" :
    //             Debug.Log("현재 재접속 가능한 방이 없습니다.");
    //             UIManager.Instance.OpenRecyclePopup("안내", "재접속이 불가능합니다 타이틀로 이동합니다.", GoTitle);
    //             break;
    //     }
    // });
}
