using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoginType
{
    Guest,
    Custom,
    Auto
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