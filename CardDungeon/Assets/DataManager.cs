using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using UnityEditor;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private static DataManager instance;
    
    public bool LoadStatus = false;
    
    public SystemLanguage systemLanguage;

    List<Dictionary<string, object>> DataDic = new List<Dictionary<string, object>>();
    
    public UserBattleInfo          userData             = new();
 
    public string TimeValue;
    
    public string ServerTimeValue;
    
    private bool isServerTimeUpdate = false;
    private bool isChangeServerTime = false;
    private float oldTime = 0;
    private DateTime serverTime;
    
    public DateTime LocalTime;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }

        instance = this;
        // 모든 씬에서 유지
        DontDestroyOnLoad(this.gameObject);
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
    
    public void Initialize()
    {
        SetData();
        DataDic.Clear();
    }
    
    public void SetData()
    {
        systemLanguage = SystemLanguage.Korean;
        
        // for (int i = 0; i < (int)ServerDataType.LastTable; i++)
        // {
        //     ChartFileNameList.Add(((ServerDataType)i).ToString());
        // }
    }
    
    public void SetDefaultData()
    {
        SetUserData();
    }

    public void SetUserData()
    {
        BackendManager.Instance.userInfo.Nickname = Backend.UserNickName;
        
        userData.LastConnect = LocalTime.ToString();
    }
    
    public void SetUserData(UserDataType type, JsonData json)
    {
        switch (type)
        {
            case UserDataType.UserLoginData:
                userData.rowIndate            = json["inDate"].ToString();
                         
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
            BackendManager.Instance.AddTransactionInsert(UserDataType.UserLoginData, paramUserData);
        else
            BackendManager.Instance.AddTransactionUpdate(UserDataType.UserLoginData, userData.rowIndate, paramUserData);
    }
    
    public void SaveAllDataAtFirst()
    {
        Debug.Log("신규 데이터 삽입");
        SaveUserBattleInfo(ServerSaveType.Insert);

        BackendManager.Instance.SendTransaction(TransactionType.Insert, this);
    }
    public void SetRowInDate(UserDataType table, string inDate)
    {
        switch (table)
        {
            case UserDataType.UserLoginData:
                userData.rowIndate          = inDate;
                break;
        }
    }
}
