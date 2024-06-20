using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoginType
{
    Custom,
    Google
}

public enum TransactionType
{
    Insert,
    Update,
    SetGet
}

public enum UserDataType
{
    UserBattleData,
    UserInvenData,
    UserAchievementData,
    UserDailyData
}

public enum TableDataType
{
    
}

public enum ServerState
{
    Online = 0,
    Offline,
    Maintenance
}

public enum GameLogType
{
    Signin,
    Login,
}

public enum ServerType
{
    Dev,
    Live
}

public enum PlatformType
{
    Window,
    Android,
    IOS
}

public enum ServerSaveType
{
    Insert,
    Update
}

public enum CurrentUIStatus
{
    UI,
    ABBPopup,
    NoneABBPopup
}