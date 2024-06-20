using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class DataManager : Singleton<DataManager>
{
    private static DataManager instance;
    
    public bool loadStatus = false;

    public List<string> chartFileNameList = new();
    
    //public SystemLanguage systemLanguage;

    List<Dictionary<string, object>> dataDic = new();

    // 유저 데이터
    public UserBattleData      userBattleData      = new();
    public UserInvenData       userInvenData       = new();
    public UserAchievementData userAchievementData = new();
    public UserDailyData       userDailyData       = new();
    
    public DateTime LocalTime;
    
    private bool isServerTimeUpdate = false;
    private bool isChangeServerTime = false;
    
    private float oldTime = 0;
    
    private DateTime serverTime;
    
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        //SetData();
        dataDic.Clear();
    }

    // 유저 초기 값 설정
    public void SetUserDefaultData()
    {
        BackendManager.Instance.userInfo.Nickname = Backend.UserNickName;
        
        // BattleData
        TotalBattleData totalData = new();
        
        totalData.totalBattleCount = 0;
        totalData.totalWinCount    = 0;
        totalData.totalWinRate     = 0;

        userBattleData.totalBattleData = totalData;
        
        for (int i = 0; i < 3; i++)
        {
            BattleData data  = new();
            data.mapIndex    = i;
            data.battleCount = 0;
            data.winCount    = 0;
            data.winRate     = 0;
            
            userBattleData.battleDataList.Add(data);
        }
        
        // InvenData
        userInvenData.isADRemoved = false;
        
        userDailyData.freeRerollTicket = false;
        userDailyData.freeADTicket     = false;

        SaveAllDataAtFirst();
    }

    private void FixedUpdate()
    {
        if (isServerTimeUpdate)
        {
            if (isChangeServerTime)
            {
                oldTime = Time.fixedUnscaledTime;
                LocalTime = serverTime;
                isChangeServerTime = false;
            }

            var delta = Time.fixedUnscaledTime - oldTime;
            oldTime = Time.fixedUnscaledTime;

            LocalTime = LocalTime.AddMilliseconds(delta * 1000.0f);
        }
    }
    
    public void SetData()
    {
        // 아이템 정보, 업적 정보, 클래스 정보, 맵 정보 테이블 불러 올것
    }

    // 각 데이터별 불러오기 처리
    public void SetUserData(UserDataType type, JsonData json)
    {
        switch (type)
        {
            case UserDataType.UserBattleData:
                userBattleData.RowIndate                        = json["inDate"].ToString();

                userBattleData.totalBattleData.totalBattleCount = int.Parse(json["TotalBattleData"]["totalBattleCount"].ToString());
                userBattleData.totalBattleData.totalWinCount    = int.Parse(json["TotalBattleData"]["totalWinCount"].ToString());
                userBattleData.totalBattleData.totalWinRate     = float.Parse(json["TotalBattleData"]["totalWinRate"].ToString());

                //if (json["BattleData"].Count <= 1) return;
                
                userBattleData.battleDataList.Clear();
                
                for (int j = 0; j < json["BattleData"].Count; j++)
                {
                    BattleData getBattleData = new();
                    
                    getBattleData.mapIndex    = int.Parse(json["BattleData"][j]["mapIndex"].ToString());
                    getBattleData.battleCount = int.Parse(json["BattleData"][j]["battleCount"].ToString());
                    getBattleData.winCount    = int.Parse(json["BattleData"][j]["winCount"].ToString());
                    getBattleData.winRate     = float.Parse(json["BattleData"][j]["winRate"].ToString());
                    
                    userBattleData.battleDataList.Add(getBattleData);
                }
                SaveUserBattleData(ServerSaveType.Update);
                break;
            case UserDataType.UserInvenData:
                userInvenData.RowIndate                 = json["inDate"].ToString();
                
                userInvenData.isADRemoved               = bool.Parse(json["isADRemoved"].ToString());

                userInvenData.itemDataList.Clear();
                
                for (int j = 0; j < json["ItemData"].Count; j++)
                {
                    ItemData getItemData   = new();
                    
                    getItemData.itemIndex  = int.Parse(json["ItemData"][j]["itemIndex"].ToString());
                    getItemData.itemAmount = int.Parse(json["ItemData"][j]["itemAmount"].ToString());
                    
                    userInvenData.itemDataList.Add(getItemData);
                }
                
                SaveUserInvenData(ServerSaveType.Update);
                break;
            case UserDataType.UserAchievementData:
                userAchievementData.RowIndate = json["inDate"].ToString();

                userAchievementData.achievementDataList.Clear();
                
                for (int j = 0; j < json["AchievementData"].Count; j++)
                {
                    AchievementData getAchievementData = new();

                    getAchievementData.achievementID   = int.Parse(json["AchievementData"][j]["achievementID"].ToString());
                    getAchievementData.currentPoint    = int.Parse(json["AchievementData"][j]["currentPoint"].ToString());
                    getAchievementData.maxPoint        = int.Parse(json["AchievementData"][j]["maxPoint"].ToString());
                    
                    userAchievementData.achievementDataList.Add(getAchievementData);
                }

                SaveUserAchievementData(ServerSaveType.Update);
                break;
            case UserDataType.UserDailyData:
                userDailyData.RowIndate = json["inDate"].ToString();

                userDailyData.freeADTicket     = bool.Parse(json["freeADTicket"].ToString());
                userDailyData.freeRerollTicket = bool.Parse(json["freeRerollTicket"].ToString());
                    
                SaveDailyData(ServerSaveType.Update);
                break;
        }
    }

    // 유저 배틀 정보 저장
    public void SaveUserBattleData(ServerSaveType type)
    {
        Param paramUserBattleData = new Param();
        
        TotalBattleData totalBattleData  = new();

        totalBattleData.totalBattleCount = userBattleData.totalBattleData.totalBattleCount;
        totalBattleData.totalWinCount    = userBattleData.totalBattleData.totalWinCount;
        totalBattleData.totalWinRate     = userBattleData.totalBattleData.totalWinRate;
        
        paramUserBattleData.Add("TotalBattleData", totalBattleData);
        
        List<BattleData> tempBattleDataList = new();
        
        for (int i = 0; i < userBattleData.battleDataList.Count; i++)
        {
            BattleData battleData  = new();
            
            battleData.mapIndex    = userBattleData.battleDataList[i].mapIndex;
            battleData.battleCount = userBattleData.battleDataList[i].battleCount;
            battleData.winCount    = userBattleData.battleDataList[i].winCount;
            battleData.winRate     = userBattleData.battleDataList[i].winRate;
            
            tempBattleDataList.Add(battleData);
        }

        paramUserBattleData.Add("BattleData", tempBattleDataList);
        
        if(type == ServerSaveType.Insert)
            BackendManager.Instance.AddTransactionInsert(UserDataType.UserBattleData, paramUserBattleData);
        else
            BackendManager.Instance.AddTransactionUpdate(UserDataType.UserBattleData, userBattleData.RowIndate, paramUserBattleData);
        
        //BackendManager.Instance.SendTransaction(TransactionType.Insert, userBattleData);
    }
    
    // 유저 인벤토리 정보 저장
    public void SaveUserInvenData(ServerSaveType type)
    {
        Param paramUserInvenData = new Param();

        List<ItemData> tempInvenDataList = new();
        
        paramUserInvenData.Add("isADRemoved", userInvenData.isADRemoved);
        
        for (int i = 0; i < userInvenData.itemDataList.Count; i++)
        {
            ItemData itemData  = new();
            
            itemData.itemIndex   = userInvenData.itemDataList[i].itemIndex;
            itemData.itemAmount  = userInvenData.itemDataList[i].itemAmount;
            
            tempInvenDataList.Add(itemData);
        }
        
        paramUserInvenData.Add("ItemData", tempInvenDataList);
        
        if(type == ServerSaveType.Insert)
            BackendManager.Instance.AddTransactionInsert(UserDataType.UserInvenData, paramUserInvenData);
        else
            BackendManager.Instance.AddTransactionUpdate(UserDataType.UserInvenData, userInvenData.RowIndate, paramUserInvenData);
        
        //BackendManager.Instance.SendTransaction(TransactionType.Insert, userInvenData);
    }
    
    // 유저 업적 달성 정보 저장
    public void SaveUserAchievementData(ServerSaveType type)
    {
        Param paramUserAchievementData = new Param();
        
        List<AchievementData> tempAchievementDataList = new();
        
        for (int i = 0; i < userAchievementData.achievementDataList.Count; i++)
        {
            AchievementData achievementData  = new();
            
            achievementData.achievementID   = userAchievementData.achievementDataList[i].achievementID;
            achievementData.currentPoint    = userAchievementData.achievementDataList[i].currentPoint;
            achievementData.maxPoint        = userAchievementData.achievementDataList[i].maxPoint;
            
            tempAchievementDataList.Add(achievementData);
        }
        
        paramUserAchievementData.Add("AchievementData", tempAchievementDataList);
        
        if(type == ServerSaveType.Insert)
            BackendManager.Instance.AddTransactionInsert(UserDataType.UserAchievementData, paramUserAchievementData);
        else
            BackendManager.Instance.AddTransactionUpdate(UserDataType.UserAchievementData, userAchievementData.RowIndate, paramUserAchievementData);
        
        //BackendManager.Instance.SendTransaction(TransactionType.Insert, userAchievementData);
    }

    public void SaveDailyData(ServerSaveType type)
    {
        Param paramDailyData = new Param();

        paramDailyData.Add("freeADTicket", userDailyData.freeADTicket);
        paramDailyData.Add("freeRerollTicket", userDailyData.freeRerollTicket);

        if(type == ServerSaveType.Insert)
            BackendManager.Instance.AddTransactionInsert(UserDataType.UserDailyData, paramDailyData);
        else
            BackendManager.Instance.AddTransactionUpdate(UserDataType.UserDailyData, userDailyData.RowIndate, paramDailyData);
        
        //BackendManager.Instance.SendTransaction(TransactionType.Insert, userDailyData);
    }
    
    public void SetRowInDate(UserDataType table, string inDate)
    {
        switch (table)
        {
            case UserDataType.UserBattleData:
                userBattleData.RowIndate      = inDate;
                break;
            case UserDataType.UserInvenData:
                userInvenData.RowIndate       = inDate;
                break;
            case UserDataType.UserAchievementData:
                userAchievementData.RowIndate = inDate;
                break;
            case UserDataType.UserDailyData:
                userDailyData.RowIndate       = inDate;
                break;
        }
    }

    //private WaitForSeconds saveDelay = new WaitForSeconds(60f);
    //private TimeSpan playerTimeOneMin = new TimeSpan(0, 1, 0);
    // private IEnumerator AutoSave()
    // {
    //     while (true)
    //     {
    //         yield return saveDelay;
    //         userInfo.PlayTime = userInfo.PlayTime.Add(playerTimeOneMin);
    //     }
    // }
    
    public void SaveAllDataAtFirst()
    {
        Debug.Log("신규 유저 기본 데이터 삽입");
        SaveUserBattleData(ServerSaveType.Insert);
        SaveUserInvenData(ServerSaveType.Insert);
        SaveUserAchievementData(ServerSaveType.Insert);
        SaveDailyData(ServerSaveType.Insert);

        BackendManager.Instance.SendTransaction(TransactionType.Insert, this);
    }

    public void SaveAllData()
    {
        Debug.Log("기존 데이터 업데이트");
        SaveUserBattleData(ServerSaveType.Update);
        SaveUserInvenData(ServerSaveType.Update);
        SaveUserAchievementData(ServerSaveType.Update);
        SaveDailyData(ServerSaveType.Insert);

        BackendManager.Instance.SendTransaction(TransactionType.Update, this);
    }

    // public float WinRateCal(int battleCount, int winCount)
    // {
    //     return 
    // }
}
