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

    public void SettingRoomUI()
    {
        
    }
}
