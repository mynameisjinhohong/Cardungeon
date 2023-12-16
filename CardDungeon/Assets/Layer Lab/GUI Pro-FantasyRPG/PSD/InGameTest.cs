using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using LitJson;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameTest : MonoBehaviour {
    MatchInGameRoomInfo currentGameRoomInfo;
    Dictionary<string, MatchUserGameRecord> inGameUserList = new Dictionary<string, MatchUserGameRecord>();

    private void LeaveMatchMaking() {
        Backend.Match.OnLeaveMatchMakingServer = (LeaveChannelEventArgs args) => {
            Debug.Log("OnLeaveMatchMakingServer - 매칭 서버 접속 종료 : " + args.ToString());
        };
        
        Debug.Log($"5-a. LeaveMatchMakingServer 매치메이킹 서버 접속 종료 요청");
        
        Backend.Match.LeaveMatchMakingServer();
    }

    public void JoinGameServer(MatchInGameRoomInfo gameRoomInfo) {
        Backend.Match.OnSessionJoinInServer = (JoinChannelEventArgs args) => {
            if (args.ErrInfo == ErrorInfo.Success) {
                Debug.Log("4-2. OnSessionJoinInServer 게임 서버 접속 성공 : " + args.ToString());
                Debug.Log("이제 게임방에 접속할 수 있습니다!");
            } else {
                Debug.LogError("4-2. OnSessionJoinInServer 게임 서버 접속 실패 : " + args.ToString());
            }
            
            // 게임 서버에 정상적으로 접속했으면 매칭 서버를 종료
            // LeaveMatchMaking();
            JoinGameRoom();
            SceneManager.LoadScene(1);
        };

        Debug.Log("4-1. JoinGameServer 인게임 서버 접속 요청");
        
        currentGameRoomInfo = gameRoomInfo;
        ErrorInfo errorInfo = null;

        if (Backend.Match.JoinGameServer(currentGameRoomInfo.m_inGameServerEndPoint.m_address, currentGameRoomInfo.m_inGameServerEndPoint.m_port, false, out errorInfo) == false) {
            Debug.LogError("JoinGameServer 중 로컬 에러가 발생했습니다." + errorInfo);
            return;
        }
    }

    public void JoinGameRoom() {
        Backend.Match.OnSessionListInServer = (MatchInGameSessionListEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Success) {
                Debug.Log("5-2. OnSessionListInServer 게임룸 접속 성공 : " + args.ToString());

                foreach (var list in args.GameRecords) {
                    if (inGameUserList.ContainsKey(list.m_nickname)) {
                        continue;
                    }
                    inGameUserList.Add(list.m_nickname, list);
                }
            } else {
                Debug.LogError("5-2. OnSessionListInServer : " + args.ToString());
            }
        };

        Backend.Match.OnMatchInGameAccess = (MatchInGameSessionEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Success) {
                Debug.Log($"5-3. OnMatchInGameAccess - 유저가 접속했습니다 : {args.GameRecord.m_nickname}({args.GameRecord.m_sessionId})");
                if (!inGameUserList.ContainsKey(args.GameRecord.m_nickname)) {
                    inGameUserList.Add(args.GameRecord.m_nickname, args.GameRecord);
                }
            } else {
                Debug.LogError("5-3. OnMatchInGameAccess : " + args.ErrInfo.ToString());
            }
        };
        
        Backend.Match.OnMatchInGameStart = () => {
            string userListString = "접속한 유저 : \n";
            foreach (var list in inGameUserList)
            {
                userListString += $"{list.Value.m_nickname}({list.Value.m_sessionId})" + (list.Value.m_isSuperGamer == true ? "슈퍼게이머" : "");

                UserData data = new UserData();
                    
                data.playerToken = list.Value.m_sessionId.ToString();
                data.playerName = list.Value.m_nickname;
                data.isSuperGamer = list.Value.m_isSuperGamer;
                
                BackendManager.Instance.UserDataList.Add(data);
            }

            Debug.Log("6-1. OnMatchInGameStart 인게임 시작");
            Debug.Log(userListString);
            Debug.Log("데이터를 보낼 수 있습니다!");
            BackendManager.Instance.isLoadGame = true;
        };
        
        Debug.Log($"5-1. JoinGameRoom 게임룸 접속 요청 : 토큰({currentGameRoomInfo.m_inGameRoomToken}");
        Backend.Match.JoinGameRoom(currentGameRoomInfo.m_inGameRoomToken);
    }
    
    // 릴레이할 데이터
    public class Message {
        public string message;
        public int x;
        public int y;
        public bool imnotsupergamer;

        public override string ToString() {
            return message;
        }
    }
    
    public void SendData() {
        if ( Backend.Match.OnMatchRelay == null) {
            Backend.Match.OnMatchRelay = (MatchRelayEventArgs args) => {
                var strByte = System.Text.Encoding.Default.GetString(args.BinaryUserData);
                Message msg = JsonUtility.FromJson<Message>(strByte);
                Debug.Log($"서버에서 받은 데이터 : {args.From.NickName} : {msg.ToString()}");
            };
        }
        
        Message message = new Message();
        //message.message = _inputField.text;
        var jsonData = JsonUtility.ToJson(message); // 클래스를 json으로 변환해주는 함수
        var dataByte = System.Text.Encoding.UTF8.GetBytes(jsonData); // json을 byte[]로 변환해주는 함수
        Backend.Match.SendDataToInGameRoom(dataByte);
    }

    public void MatchEnd() {
        Backend.Match.OnLeaveInGameServer = (MatchInGameSessionEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Success) {
                Debug.Log("OnLeaveInGameServer 인게임 서버 접속 종료 : " + args.ErrInfo.ToString());
            } else {
                Debug.LogError("OnLeaveInGameServer 인게임 서버 접속 종료 : " + args.ErrInfo + " / " + args.Reason);
            }
        };
        
        Backend.Match.OnMatchResult = (MatchResultEventArgs args) => {
            if (args.ErrInfo == ErrorCode.Success) {
                Debug.Log("8-2. OnMatchResult 성공 : " + args.ErrInfo.ToString());
            } else {
                Debug.LogError("8-2. OnMatchResult 실패 : " + args.ErrInfo.ToString());
            }
        };        
        Debug.Log("8-1. MatchEnd 호출");
        MatchGameResult matchGameResult = new MatchGameResult();
        matchGameResult.m_winners = new List<SessionId>();
        matchGameResult.m_losers = new List<SessionId>();
        
        foreach (var session in inGameUserList) {
            // 순서는 무관합니다.
            matchGameResult.m_winners.Add(session.Value.m_sessionId);
        }
        
        Backend.Match.MatchEnd(matchGameResult);
    }
}