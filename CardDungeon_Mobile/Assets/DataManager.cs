using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using UnityEditor;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public bool LoadStatus = false;
    
    public SystemLanguage systemLanguage;

    List<Dictionary<string, object>> DataDic = new List<Dictionary<string, object>>();
    
    public UserBattleInfo          userData             = new();
 
    public DateTime LocalTime;
    
    public string TimeValue;
    
    public string ServerTimeValue;
    
    private bool isServerTimeUpdate = false;
    private bool isChangeServerTime = false;
    private float oldTime = 0;
    private DateTime serverTime;
    
    public void Initialize()
    {
        SetData();
        DataDic.Clear();
    }

    public void SetDefaultData()
    {
        SetUserData();
    }
    
    public void SetLocalTime(DateTime servertime)
    {
        if (serverTime < servertime)
        {
            Debug.Log("시간 업데이트 확인" + servertime);
            serverTime = servertime;
            userData.LastConnect = serverTime.ToString();
            isServerTimeUpdate = true;
            isChangeServerTime = true;
        }
        else
        {
            Debug.Log("조건 틀려서 업데이트 안함");
        }
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

            TimeValue = LocalTime.ToString();

            ServerTimeValue = serverTime.ToString();
        }
    }

    public void SetData()
    {
        systemLanguage = SystemLanguage.Korean;
        
        // for (int i = 0; i < (int)ServerDataType.LastTable; i++)
        // {
        //     ChartFileNameList.Add(((ServerDataType)i).ToString());
        // }
    }

    public void SetUserData()
    {
        BackendManager.Instance.userInfo.Nickname = Backend.UserNickName;
        
        userData.LastConnect = LocalTime.ToString();
        userData.totalBattleCount = 0;
        userData.winCount = 0;
        userData.winRate = 0;
    }
    
    public void SetUserData(UserDataType type, JsonData json)
    {
        switch (type)
        {
            case UserDataType.UserBattleInfo:
                userData.RowIndate            = json["inDate"].ToString();
                         
                userData.totalBattleCount     = Int32.Parse(json["totalBattleCount"].ToString());
                userData.winCount             = Int32.Parse(json["winCount"].ToString());
                userData.winRate              = float.Parse(json["winRate"].ToString());

                userData.LastConnect          = json["LastConnect"].ToString();
                break;
        }
    }

    public void SaveUserBattleInfo(ServerSaveType type)
    {
        userData.LastConnect = LocalTime.ToString("yyyy-MM-dd HH:mm:ss");
        Param paramUserData = new Param();
        paramUserData.Add("totalBattleCount", userData.totalBattleCount);
        paramUserData.Add("winCount", userData.winCount);
        paramUserData.Add("winRate", userData.winRate);
        paramUserData.Add("LastConnect", userData.LastConnect);
        if(type == ServerSaveType.Insert)
            BackendManager.Instance.AddTransactionInsert(UserDataType.UserBattleInfo, paramUserData);
        else
            BackendManager.Instance.AddTransactionUpdate(UserDataType.UserBattleInfo, userData.RowIndate, paramUserData);
    }
    
    public void SaveAllDataAtFirst()
    {
        Debug.Log("신규 데이터 삽입");
        SaveUserBattleInfo(ServerSaveType.Insert);

        BackendManager.Instance.SendTransaction(TransactionType.Insert, userData);
    }
    public void SetRowInDate(UserDataType table, string inDate)
    {
        switch (table)
        {
            case UserDataType.UserBattleInfo:
                userData.RowIndate          = inDate;
                break;
        }
    }
    
    
    public void DataLoadComplete()
    {
        // 푸시 정보 설정
        // SetupManager.Instance.RegistPush();
        // SetOffLineReward(DateTime.Parse(userData.LastConnect));
        // CheckMissionRefresh();
        
        //StartCoroutine(nameof(AutoSave));
    }
    
    private WaitForSeconds saveDelay = new WaitForSeconds(60f);
    private TimeSpan playerTimeOneMin = new TimeSpan(0, 1, 0);
    // private IEnumerator AutoSave()
    // {
    //     while (true)
    //     {
    //         yield return saveDelay;
    //         userInfo.PlayTime = userInfo.PlayTime.Add(playerTimeOneMin);
    //     }
    // }
}
