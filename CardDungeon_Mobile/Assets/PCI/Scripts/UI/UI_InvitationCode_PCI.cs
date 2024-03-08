using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InvitationCode_PCI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Button btn_Paste;
    public TMPro.TMP_InputField inputField;
    public Button btn_Confirm, btn_Cancel;

    // Start is called before the first frame update
    void Start()
    {
        btn_Paste.onClick.AddListener(Paste);
        btn_Confirm.onClick.AddListener(Confirm);
        btn_Cancel.onClick.AddListener(Cancel);
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

    private void Paste()
    {
        inputField.text =  GUIUtility.systemCopyBuffer;
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
