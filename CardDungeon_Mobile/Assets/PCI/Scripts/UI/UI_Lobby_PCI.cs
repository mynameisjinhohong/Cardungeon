using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lobby_PCI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public List<UI_UserIDPanel_PCI> slots = new List<UI_UserIDPanel_PCI>();

    public TMP_Text roomNameText;
    
    public TMPro.TextMeshProUGUI invitationCode, userCount;

    public Button btn_MatchStart, btn_ExitRoom;

    private string TODO_lobbyData;

    // Start is called before the first frame update
    void Start()
    {
        btn_MatchStart.onClick.AddListener(MatchStart);
        btn_ExitRoom.onClick.AddListener(ExitRoom);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void MatchStart()
    {
        // process
    }

    private void ExitRoom()
    {
        // process
        Hide();
    }

    private void SetLobbyData(string TODO_lobbyDataContainer)
    {

    }

    // TODO : ???? ?????? ??????? ???
    private void AddUser(string TODO_userDataContainer)
    {
        foreach(var e in slots)
        {
            if (e.isEmpty)
            {
                e.SetUserData(TODO_userDataContainer);
            }
        }
    }

    // TODO : ???? ?????? ??????? ???
    private void RemoveUser(string TODO_userDataContainer)
    {
        foreach(var e in slots)
        {
            if(e.TODO_userDataContainer == TODO_userDataContainer)
            {
                e.ClearUserData();
            }
        }
    }

    private void CopyInvitaionCode()
    {
        GUIUtility.systemCopyBuffer = "gangtoesal@gmail.com";
    }
}
