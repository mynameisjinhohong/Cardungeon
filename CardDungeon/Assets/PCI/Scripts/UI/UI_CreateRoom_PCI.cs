using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreateRoom_PCI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TMPro.TextMeshProUGUI txt_InvitaionCode;
    public Button btn_copyInvitationCode;
    public TMPro.TMP_InputField inputField_RoomName;
    public Button btn_left, btn_right;
    public TMPro.TextMeshProUGUI txt_UserCount;
    public Button btn_Confirm, btn_Cancel;

    private int m_userCount = 4;
    private int UserCount
    {
        get { return m_userCount; }
        set
        {
            if (value < 0) return;
            txt_UserCount.text = $"{value}";
            m_userCount = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        btn_copyInvitationCode.onClick.AddListener(CopyInvitationCode);
        btn_left.onClick.AddListener(UserCountLeft);
        btn_right.onClick.AddListener(UserCountRight);
        btn_Confirm.onClick.AddListener(Confirm);
        btn_Cancel.onClick.AddListener (Cancel);
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

    private void CopyInvitationCode()
    {
        GUIUtility.systemCopyBuffer = txt_InvitaionCode.text;
    }

    private void UserCountLeft()
    {
        UserCount--;
    }

    private void UserCountRight()
    {
        UserCount++;
    }

    private void Confirm()
    {
        Hide();
    }

    private void Cancel()
    {
        Hide();
    }
}
