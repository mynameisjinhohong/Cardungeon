using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEditor;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private static DataManager instance;
    
    public bool LoadStatus = false;
    
    List<Dictionary<string, object>> DataDic = new List<Dictionary<string, object>>();
    
    public UserLoginData          userData             = new();
 
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

    public void Initialize()
    {
        //SetData();
        DataDic.Clear();
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

    public void SaveUserData(SaveType type)
    {
        userData.LastConnect = LocalTime.ToString("yyyy-MM-dd HH:mm:ss");
        Param paramUserData = new Param();
        // paramUserData.Add("Zeny", userData.Zeny.GetDecrypted());
        // paramUserData.Add("Gem", userData.Gem.GetDecrypted());
        // paramUserData.Add("Energy", userData.Energy.GetDecrypted());
        // paramUserData.Add("LastConnect", userData.LastConnect.GetDecrypted());
        // if(type == SaveType.Insert)
        //     BackendManager.Instance.AddTransactionInsert(UserDataType.UserData, paramUserData);
        // else
        //     BackendManager.Instance.AddTransactionUpdate(UserDataType.UserData, userData.RowIndate, paramUserData);
    }
}
