using System;
using BackEnd;
using BackEnd.Tcp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    #region 호스트
    public Queue<Message> messageQueue;
    #endregion
    public List<Color> colorList;

    public bool isUseChaser;
    public bool isSoloTest;
    public GameObject chaserObj;
    public Chaser chaser;
    public bool isWaiting;

    public int classSelectedUser;

    public int errorCount;
    
    //맵 생성 디버깅용값
    public int testValue;
    
    // Start is called before the first frame update
    void Start()
    {
        classSelectedUser = 0;
        
        BackendManager.Instance.DoClassChoiceTime();

        // 넘어온 정보를 토대로 유저 리스트 인덱스 정렬
        BackendManager.Instance.userDataList.Sort((UserData lhs, UserData rhs) =>
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
        
        // 정렬된 인덱스로 플레이어 생성
        DataInit();
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

    public void DataInit()
    {
        errorCount = 0;
        
        players = new List<Player_HJH>();
        for (int i = 0; i < playerPrefabs.Count; i++)
        {
            if (i < BackendManager.Instance.userDataList.Count)
            {
                GameObject PlayerPrefab = Instantiate(playerPrefabs[i], PlayerSpawnPosition[i]);
                Player_HJH playerHjh = PlayerPrefab.GetComponent<Player_HJH>();
                players.Add(playerHjh);
                playerHjh.isSuperGamer = BackendManager.Instance.userDataList[i].isSuperGamer;
                playerHjh.PlayerName.text = BackendManager.Instance.userDataList[i].playerName;

                if (BackendManager.Instance.userInfo.Nickname == BackendManager.Instance.userDataList[i].playerName)
                {
                    playerHjh.isMine = true;
                    mainUi.myPlayer = playerHjh;
                    mainUi.myPlayer.isMeIconObj.SetActive(true);
                    mainUi.myPlayer.KeysOnValueChanged += mainUi.SetKeysUI;
                }
                playerHjh.PlayerToken = BackendManager.Instance.userDataList[i].playerToken;

                if (BackendManager.Instance.userDataList[i].isSuperGamer)
                {
                    SuperGamerIdx = i;
                }

                players[i].gameObject.SetActive(true);
            }

        }

        for (int i = 0; i < BackendManager.Instance.userDataList.Count; i++)
        {
            if (BackendManager.Instance.userInfo.Nickname == BackendManager.Instance.userDataList[i].playerName)
            {
                myIdx = i;

                Transform parentTransform = PlayerSpawnPosition[myIdx].transform;

                if(Camera.main.TryGetComponent<CameraManager_HJH>(out var cam))
                {
                    cam.target = parentTransform.GetChild(0);
                    cam.transform.position = cam.target.position + new Vector3(0.5f, 0.5f, -10f);
                }
                else
                {
                    Camera.main.transform.SetParent(parentTransform.GetChild(0));

                    Camera.main.transform.localPosition = new Vector3(0.5f, 0.5f, -10f);
                }
            }
        }

        mainUi.playerBG.color = colorList[myIdx];
        
        // 플레이어 생성 완료 후 맵 생성 준비
        InitializeGame();
    }
    // public bool CardIdxCheckNoPlayer(int cardIdx, Transform playerPos)
    // {
    //     Vector2 goPos = new Vector2();
    //     switch (Mathf.Abs(cardIdx))
    //     {
    //         case 1:
    //             if(cardIdx > 0)
    //             {
    //                 goPos = new Vector2(playerPos.position.x, playerPos.position.y + 2);
    //             }
    //             else
    //             {
    //                 goPos = new Vector2(playerPos.position.x, playerPos.position.y + 1);
    //             }
    //             break;
    //         case 2:
    //             if (cardIdx > 0)
    //             {
    //                 goPos = new Vector2(playerPos.position.x + 2, playerPos.position.y);
    //             }
    //             else
    //             {
    //                 goPos = new Vector2(playerPos.position.x + 1, playerPos.position.y);
    //             }
    //             break;
    //         case 3:
    //             if (cardIdx > 0)
    //             {
    //                 goPos = new Vector2(playerPos.position.x - 2, playerPos.position.y);
    //             }
    //             else
    //             {
    //                 goPos = new Vector2(playerPos.position.x - 1, playerPos.position.y);
    //             }
    //             break;
    //         case 4:
    //             if (cardIdx > 0)
    //             {
    //                 goPos = new Vector2(playerPos.position.x, playerPos.position.y - 2);
    //             }
    //             else
    //             {
    //                 goPos = new Vector2(playerPos.position.x, playerPos.position.y - 1);
    //             }
    //             break;
    //         case 5:
    //             if (cardIdx > 0)
    //             {
    //                 goPos = new Vector2(playerPos.position.x, playerPos.position.y - 2);
    //             }
    //             else
    //             {
    //                 goPos = new Vector2(playerPos.position.x, playerPos.position.y - 1);
    //             }
    //             break;
    //     }
    //     if(Mathf.Abs(cardIdx) == 6)
    //     {
    //         if(cardIdx > 0)
    //         {
    //             Random.InitState(CardManager.Instance.seed);
    //             int x = Random.Range(-2, 3);
    //             int y = Random.Range(-2, 3);
    //             if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)playerPos.transform.position.x + x, (int)playerPos.transform.position.y + y)))
    //             {
    //                 goPos = new Vector3(playerPos.transform.position.x + x, playerPos.transform.position.y + y, playerPos.transform.position.z);
    //             }
    //             else
    //             {
    //                 for (int i = -2; i < 3; i++)
    //                 {
    //                     for (int j = -2; j < 3; j++)
    //                     {
    //                         if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)playerPos.transform.position.x + j, (int)playerPos.transform.position.y + i)))
    //                         {
    //                             goPos = new Vector3(playerPos.transform.position.x + j, playerPos.transform.position.y + i, playerPos.transform.position.z);
    //                         }
    //                     }
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             Random.InitState(CardManager.Instance.seed);
    //             int x = Random.Range(-3, 4);
    //             int y = Random.Range(-3, 4);
    //             if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)playerPos.transform.position.x + x, (int)playerPos.transform.position.y + y)))
    //             {
    //                 goPos = new Vector3(playerPos.transform.position.x + x, playerPos.transform.position.y + y, playerPos.transform.position.z);
    //             }
    //             else
    //             {
    //                 for (int i = -3; i < 4; i++)
    //                 {
    //                     for (int j = -3; j < 4; j++)
    //                     {
    //                         if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)playerPos.transform.position.x + j, (int)playerPos.transform.position.y + i)))
    //                         {
    //                             goPos = new Vector3(playerPos.transform.position.x + j, playerPos.transform.position.y + i, playerPos.transform.position.z);
    //                         }
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //     return CheckNoPlayer(goPos);
    // }
    // public bool CheckNoPlayer(Vector2 goPos)
    // {
    //     for (int i = 0; i < players.Count; i++)
    //     {
    //         Vector2 vec = new Vector2((int)players[i].transform.position.x, (int)players[i].transform.position.y);
    //         if ((Vector2)goPos == vec)
    //         {
    //             return false;
    //         }
    //     }
    //     return true;
    // }

    public void InitializeGame()
    {
        BackendManager.Instance.userGradeList = new List<MatchUserGameRecord>();

        // 서버에서 오는 메세지 수신할 준비
        Backend.Match.OnMatchRelay = (MatchRelayEventArgs args) => //서버로 보낸 메세지를 클라이언트에 콜백했을 때 호출되는 이벤트
            {
                // 내가 보낸 메세지는 무시 처리
                if (args.From.NickName == BackendManager.Instance.userInfo.Nickname)
                {
                    Debug.Log("내가 보낸 메세지 무시");
                    return;
                }

                var strByte = System.Text.Encoding.Default.GetString(args.BinaryUserData);
                Message msg = JsonUtility.FromJson<Message>(strByte);

                if (BackendManager.Instance.isMeSuperGamer)
                {
                    Debug.Log("슈퍼게이머인 나는 메세지를 송신 합니다");
                    messageQueue.Enqueue(msg);

                    // 슈퍼게이머가 패배 처리 요청 받으면 모든 유저에게 정보 전송
                    if (msg.playerIdx == -99)
                    {
                        if (BackendManager.Instance.winUser == "")
                        {
                            Debug.Log("승리유저 정보 비어있어서 리턴 처리");
                            return;
                        }
                            
                        Debug.Log($"{BackendManager.Instance.winUser}승리 처리 메세지 수신");
                        BackendManager.Instance.SendResultToServer();
                    }
                    // 유저가 클래스 선택하면 슈퍼게이머도 수신
                    else if (msg.cardIdx <= -100)
                    {
                        Debug.Log($"{msg.playerIdx}님이 {msg.cardIdx} 클래스 선택");
                    }
                }
                else
                {
                    Debug.Log("슈퍼게이머가 아닌  나는 메세지를 수신합니다");
                    //지금 받은게 만약에 슈퍼게이머가 보낸거면
                    if (args.From.NickName == BackendManager.Instance.userDataList[SuperGamerIdx].playerName) 
                    {
                        if (msg.playerIdx == -10) //-10 플레이어 인덱스를 받으면 맵을 생성한다.
                        {
                            int head = BackendManager.Instance.userDataList.Count;
                            
                            Debug.Log($"맵생성 수신{head}");
                            
                            switch (head)
                            {
                                case 1:
                                    gameBoard.Generate(msg.cardIdx, 12, 12, 2);
                                    break;
                                case 2:
                                    gameBoard.Generate(msg.cardIdx, 12, 12, 2);
                                    break;
                                case 3:
                                case 4:
                                    gameBoard.Generate(msg.cardIdx, 14, 14, 4);
                                    break;
                                case 5:
                                case 6:
                                case 7:
                                case 8:
                                    gameBoard.Generate(msg.cardIdx, 16, 16, 8);
                                    break;
                                default:
                                    UIManager.Instance.OpenRecyclePopup("시스템 에러", "맵 생성 실패", Application.Quit);
                                    break;
                            }
                            CardManager.Instance.seed = msg.cardIdx;
                        }
                        else if (msg.cardIdx <= -100)
                        {
                            Debug.Log($"{msg.playerIdx}님이 {msg.cardIdx} 클래스 선택");
                        }
                        else if (msg.playerIdx <= 9)
                        {
                            Debug.Log(msg.playerIdx + "  " + msg.cardIdx);
                            CardRealGo(msg.playerIdx, msg.cardIdx); //특정 카드를 사용한 플레이어를 받아서 실제 실행
                        }
                    }
                    else
                    {
                        Debug.Log("슈퍼게이머가 아닌 유저에게 수신함");
                    }
                }
                //Debug.Log($"서버에서 받은 데이터 : {args.From.NickName} : {msg.ToString()}");
            };
        //gameRecord = new Stack<SessionId>();
        //GameManager.OnGameOver += OnGameOver;
        //GameManager.OnGameResult += OnGameResult;
        //myPlayerIndex = SessionId.None;
        //SetPlayerAttribute();
        //OnGameStart();
        
        Backend.Match.OnSessionOffline = (MatchInGameSessionEventArgs args) => {
            
            Debug.Log(args.GameRecord.m_nickname + "님이 연결을 종료하셨습니다.");

            int OutUserIndex = 0;
            
            for (int i = 0; i < BackendManager.Instance.userDataList.Count; i++)
            {
                if (BackendManager.Instance.userDataList[i].playerName == args.GameRecord.m_nickname)
                {
                    OutUserIndex = i;
                }
            }

            //BackendManager.Instance.userDataList.RemoveAt(OutUserIndex);
            //Destroy(PlayerSpawnPosition[OutUserIndex].gameObject);
            
            // 나간유저 hp 0 처리, 비활성화
            players[OutUserIndex].hp = 0;
            PlayerSpawnPosition[OutUserIndex].gameObject.SetActive(false);
            
            foreach (var userData in BackendManager.Instance.inGameUserList)
            {
                if (userData.Value.m_nickname == players[OutUserIndex].PlayerName.text)
                {
                    Debug.Log($"{userData.Value.m_nickname}님이 탈주하여 패배처리");
                    
                    // 이미 패배한 유저라면 리스트에 넣지 않음
                    if(!BackendManager.Instance.userGradeList.Contains(userData.Value))
                        BackendManager.Instance.userGradeList.Add(userData.Value);
                    
                    Debug.Log(BackendManager.Instance.userGradeList.Count + "리스트 크기확인");
                }
            }
            
            // 승리 조건 체크
            CheckAllUsersHP();
            
            // 살아남은 유저가 한명일 경우 살아있는 유저를 결과처리 리스트에 넣고 슈퍼게이머에게 게임 종료 요청
            //if (BackendManager.Instance.userDataList.Count <= 1)
            //{
            //    BackendManager.Instance.winUser = BackendManager.Instance.userDataList[0].playerName; 
            //    Debug.Log("다른유저가 모두 나가서 승리처리");
            //    foreach (var userData in BackendManager.Instance.inGameUserList)
            //    {
            //        if (userData.Value.m_nickname == BackendManager.Instance.userInfo.Nickname)
            //        {
            //            BackendManager.Instance.userGradeList.Add(userData.Value);
            //            BackendManager.Instance.winUser = BackendManager.Instance.userInfo.Nickname;
            //        }
            //    }
            //    
            //    BackendManager.Instance.SendResultToServer();
            //    SendToSuperGamerEndGame();
            //}
        };
        
        Backend.Match.OnChangeSuperGamer = (MatchInGameChangeSuperGamerEventArgs args) => {
            
            Debug.Log($"슈퍼 게이머였던 {args.OldSuperUserRecord}님이 접속을 종료하여 슈퍼게이머를 재선정합니다");
            
            for (int i = 0; i < BackendManager.Instance.userDataList.Count; i++)
            {
                if (BackendManager.Instance.userDataList[i].playerName == args.NewSuperUserRecord.m_nickname)
                {
                    Debug.Log("새로운 슈퍼게이머는" + BackendManager.Instance.userDataList[i].playerName + "님 입니다");
        
                    BackendManager.Instance.userDataList[i].isSuperGamer = true;

                    SuperGamerIdx = i;
                    
                    if (BackendManager.Instance.userDataList[i].playerName == BackendManager.Instance.userInfo.Nickname)
                    {
                        BackendManager.Instance.isMeSuperGamer = true;
                    }
                    else
                    {
                        BackendManager.Instance.isMeSuperGamer = false;
                    }
                }
                else
                {
                    BackendManager.Instance.userDataList[i].isSuperGamer = false;
                }
            }
            
            UIManager.Instance.OpenIndicator();
        };
        
        // 게임 종료후 서버 연결 끝났을때 호출
        Backend.Match.OnLeaveInGameServer = (MatchInGameSessionEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Success) {
                Debug.Log("OnLeaveInGameServer 인게임 서버 접속 종료 : " + args.ErrInfo.ToString());
            } else {
                Debug.LogError("OnLeaveInGameServer 인게임 서버 접속 종료 : " + args.ErrInfo + " / " + args.Reason);
            }
        };
        
        // 서버로 결과를 전송 완료하여 게임을 종료처리
        Backend.Match.OnMatchResult = (MatchResultEventArgs args) =>
        {
            if (args.ErrInfo == ErrorCode.Success)
            {
                Debug.Log("8-2. OnMatchResult 성공 : " + args.ErrInfo.ToString());
                GameResult(BackendManager.Instance.winUser == BackendManager.Instance.userInfo.Nickname);
            }
            else
            {
                Debug.LogError($"8-2. OnMatchResult 실패, 코드 : {args.ErrInfo} 이유 : {args.Reason}");
                
                foreach (var userData in BackendManager.Instance.inGameUserList)
                {
                    Debug.Log("실제유저 데이터 : " + userData.Value.m_nickname);
                }

                foreach (var grade in BackendManager.Instance.userGradeList)
                {
                    Debug.Log("입력된 결과 데이터 : " + grade.m_nickname);   
                }
            }
        };
    }
    public void CheckAllUsersHP()
    {
        int remainPlayerCount = 0;

        string remainPlayerNickname = "";
        
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].hp >= 1)
            {
                remainPlayerNickname = players[i].PlayerName.text;
                
                remainPlayerCount++;
            }
        }

        if (remainPlayerCount <= 1)
        {
            Debug.Log("남은 유저가 한명이라 종료 처리");
            
            BackendManager.Instance.winUser = remainPlayerNickname;

            // 다나가고 마지막 남은 유저가 슈퍼게이머라면 서버로 결과 바로 전송
            if (BackendManager.Instance.isMeSuperGamer)
            {
                foreach (var userData in BackendManager.Instance.inGameUserList)
                {
                    // 승리유저와 마지막 남은 유저의 닉네임이 같은지 체크
                    if (userData.Value.m_nickname == BackendManager.Instance.winUser)
                    {
                        // 마지막에 패배리스트에 넣음
                        if(!BackendManager.Instance.userGradeList.Contains(userData.Value))
                            BackendManager.Instance.userGradeList.Add(userData.Value);
                        
                        Debug.Log(BackendManager.Instance.userGradeList.Count + "마지막 남은유저가 보내는 패배처리 리스트크기");
                    }
                }
                
                BackendManager.Instance.SendResultToServer();
            }
            else
            {
                // 슈퍼게이머가 관전중인 상태에 승리처리
                SendToSuperGamerEndGame();
            }
        }
    }
    
    public void CreateMap()
    {
        // 게임 시작시 슈퍼게이머는 방을 생성하라는 메세지를 보냄
        if (BackendManager.Instance.isMeSuperGamer)
        {
            messageQueue = new Queue<Message>();
            
            Message m = new Message();
            m.playerIdx = -10;
            m.cardIdx = Random.Range(0, 100);
            SendData(m);
            CardManager.Instance.seed = m.cardIdx;
            int head = BackendManager.Instance.userDataList.Count;
            switch (head)
            {
                case 1:
                    gameBoard.Generate(m.cardIdx, 12, 12, 2);
                    break;
                case 2:
                    gameBoard.Generate(m.cardIdx, 12, 12, 2);
                    break;
                case 3:
                case 4:
                    gameBoard.Generate(m.cardIdx, 14, 14, 4);
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                    gameBoard.Generate(m.cardIdx, 16, 16, 8);
                    break;
                default:
                    UIManager.Instance.OpenRecyclePopup("시스템 에러", "맵 생성 실패", Application.Quit);
                    break;
            }
        }
    }
    
    public void SendToSuperGamerEndGame()
    {
        Debug.Log("승리처리 메세지 슈퍼 게이머에게 송신");
        Message m = new Message();
        m.playerIdx = -99;
        m.cardIdx = -99;
        SendData(m);
    }

    public void newSuperGamerMessageQueueInit()
    {
        messageQueue = new Queue<Message>();
    }
    
    void Update()
    {
        if (BackendManager.Instance != null)
        {
            if (BackendManager.Instance.isMeSuperGamer)
            {
                if (messageQueue == null)
                {
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

    [Serializable]
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
            //if (players.Count <= 1) return;
            
            CardManager.Instance.OnCardStart(players[playerIdx].transform, cardIdx);
        }
    }

    public void GoDamage(Vector2Int pos, int damage)
    {
        if (!BackendManager.Instance.isInitialize) return;
        
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

        CheckAllUsersHP();
    }

    public void GameResult(bool isWin)
    {
        if (isWin)
        {
            string allKillWin = "승리 하였습니다!";

            string esacpeWin = "탈출 성공!";

            mainUi.winTypeText.text = BackendManager.Instance.isEscapeWin ? esacpeWin : allKillWin;
        
            mainUi.winImage.SetActive(true);
            mainUi.gameOver.SetActive(false);
        }
        else
        {
            if(Camera.main.TryGetComponent<CameraManager_HJH>(out var cam))
            {
                cam.target = null;
            }
            else
            {
                Camera.main.transform.SetParent(null);
            }
            players[myIdx].gameObject.SetActive(false);
            
            mainUi.gameOver.SetActive(true);
            mainUi.winImage.SetActive(false);
            
            mainUi.lookAroundBtn.interactable = BackendManager.Instance.winUser == "";
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
