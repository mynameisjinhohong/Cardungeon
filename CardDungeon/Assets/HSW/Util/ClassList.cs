using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class UserDailyData
{
    public string RowIndate;
    
    public bool freeADTicket;
    public bool freeRerollTicket;
}

[Serializable]
public class UserBattleData
{
    public string RowIndate;

    public TotalBattleData totalBattleData;

    public List<BattleData> battleDataList;
}

[Serializable]
public class UserInvenData
{
    public string RowIndate;

    public bool isADRemoved;

    public List<ItemData> itemDataList;
}

[Serializable]
public class UserAchievementData
{
    public string RowIndate;

    public List<AchievementData> achievementDataList;
}

[Serializable]
public class BattleData
{
    public int mapIndex;
    public int battleCount;
    public int winCount;
    public float winRate;
}

[Serializable]
public class TotalBattleData
{
    public int totalBattleCount;
    public int totalWinCount;
    public float totalWinRate;
}

[Serializable]
public class ItemData
{
    public int itemIndex;
    public int itemAmount;
}

[Serializable]
public class AchievementData
{
    public int achievementID;
    public int currentPoint;
    public int maxPoint;
}

[Serializable]
public class ItemTableData
{
    public int itemIndex;
    public string itemName;
    public bool isInvenView;
    public string itemGrade;
}