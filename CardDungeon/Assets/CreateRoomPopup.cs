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
    private TMP_Text roomCode;

    [SerializeField]
    private TMP_InputField settedRoomName;

    [SerializeField]
    private TMP_Text settedMaxCount;

    [SerializeField]
    private int roomTypeIndex = 0;

    public int currentValue = 2;

    public void SettingRoomType(int index)
    {
        roomTypeIndex = index;
    }

    void Start()
    {
        BackendManager.Instance.GetMatchList();
        
        //roomCode.text = GenerateAuthenticationCode();

        settedData.roomName = roomCode.text;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CancelButton();
    }

    public void CreateRoomWithSettingData()
    {
        settedData.roomName = settedRoomName.text;
        settedData.roomIndexNum = FindRoomIndex(settedMaxCount.text);
        settedData.roomHeadCount = Int32.Parse(settedMaxCount.text);

        BackendManager.Instance.roomSettingData = settedData;

        MatchController.Instance.CreateRoom();

        MatchController.Instance.SelfDataInit();
        
        RoomValueInit();
    }

    public void UpDownButtonClick(bool isUp)
    {
        if (isUp && currentValue < 8)
        {
            currentValue++;
        }
        else if (!isUp && currentValue > 2)
        {
            currentValue--;
        }
        
        settedMaxCount.text = currentValue.ToString();
    }

    private int FindRoomIndex(string currentRoomIndex)
    {
        int exValue = Int32.Parse(currentRoomIndex);
        
        for (int i = 0; i < BackendManager.Instance.matchCardList.Count; i++)
        {
            if (exValue == BackendManager.Instance.matchCardList[i].matchHeadCount)
            {
                return BackendManager.Instance.matchIndex = i;
            }
        }

        return 0;
    }

    public void CancelButton()
    {
        UIManager.Instance.PopupListPop();
    }
    
    // private string GenerateAuthenticationCode()
    // {
    //     // A~Z������ ���ڿ�
    //     string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    //
    //     // �������� ������ ������ȣ
    //     string authenticationCode = "";
    //
    //     // Unity�� ���� �Լ� ���
    //     System.Random random = new System.Random();
    //
    //     for (int i = 0; i < 8; i++)
    //     {
    //         // 0���� 25������ ���� �ε����� �� A~Z �� �ϳ��� ����
    //         char randomChar = alphabet[random.Next(0, 26)];
    //
    //         // Ȧ�� �ڸ����� ���ĺ�, ¦�� �ڸ����� 1~9 �� �ϳ��� �����Ͽ� �߰�
    //         if (i % 2 == 0)
    //         {
    //             authenticationCode += randomChar;
    //         }
    //         else
    //         {
    //             // 1���� 9������ ���� �� �ϳ��� �����Ͽ� �߰�
    //             authenticationCode += random.Next(1, 10).ToString();
    //         }
    //     }
    //
    //     return authenticationCode;
    // }
    //
    // public void CopyToClipboard()
    // {
    //     // GUIUtility.systemCopyBuffer�� ����Ͽ� Ŭ�����忡 ���ڿ� ����
    //     GUIUtility.systemCopyBuffer = roomCode.text;
    // }

    private void RoomValueInit()
    {
        UI_Lobby_PCI roomUIData = MatchController.Instance.uIList[2].GetComponent<UI_Lobby_PCI>();

        if (settedRoomName.text == "")
        {
            roomUIData.roomNameText.text = BackendManager.Instance.userInfo.Nickname + "�� ��";
            
            BackendManager.Instance.roomSettingData.roomName = roomUIData.roomNameText.text;
        }
        else
        {
            roomUIData.roomNameText.text = settedRoomName.text;

            BackendManager.Instance.roomSettingData.roomName = roomUIData.roomNameText.text;
        }
            
    }
}
