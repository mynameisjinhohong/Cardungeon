using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateRoomPopup : MonoBehaviour
{
    [SerializeField]
    private RoomSettingData settedData;

    [SerializeField]
    private TMP_InputField settedRoomName;

    [SerializeField]
    private TMP_InputField settedMaxCount;

    [SerializeField]
    private int roomTypeIndex = 0;

    public void SettingRoomType(int index)
    {
        roomTypeIndex = index;
    }

    public void CreateRoomWithSettingData()
    {
        settedData.roomName = settedRoomName.text;
        settedData.maxCount = Int32.Parse(settedMaxCount.text);
        settedData.roomType = (RoomType)roomTypeIndex;

        BackendManager.Instance.roomSettingData = settedData;

        MatchController.Instance.CreateRoom();
    }

    public void UpDownButtonClick(bool isUp)
    {
        
    }
}
