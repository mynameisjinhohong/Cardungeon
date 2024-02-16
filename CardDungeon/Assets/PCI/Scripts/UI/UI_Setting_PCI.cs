using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class UI_Setting_PCI : MonoBehaviour
{
    private enum ScreenMode
    {
        Windowed,
        Fullscreen
    }
    public CanvasGroup canvasGroup;
    public TMPro.TextMeshProUGUI txt_ScreenModeTxt;
    public Button btn_ScreenModeLeft, btn_ScreenModeRight;
    public TMPro.TextMeshProUGUI txt_ResolutionTxt;
    public Button btn_ResolutionLeft, btn_ResolutionRight;
    public Slider sld_BgmSlider, sld_SfxSlider;
    public Button btn_Confirm, btn_Cancel;
    public Button btn_CopyEmail;

    private ScreenMode screenMode;
    private List<Resolution> resolutions;
    private int screenModeIdx = 0;
    private int ScreenModeIdx
    {
        get { return screenModeIdx; }
        set
        {
            screenModeIdx = value;
            switch (value)
            {
                case (int)ScreenMode.Windowed:
                    txt_ScreenModeTxt.text = "창모드";
                    break;
                case (int)ScreenMode.Fullscreen:
                    txt_ScreenModeTxt.text = "전체화면";
                    break;
            }
        }
    }
    private int resolutionIdx = 0;
    private int ResolutionIdx
    {
        get { return resolutionIdx; }
        set
        {
            try
            {
                resolutionIdx = value;
                txt_ResolutionTxt.text = resolutions[resolutionIdx].ToString();
            }
            catch(Exception ex)
            {
                resolutionIdx = Mathf.Clamp(value, 0, resolutions.Count - 1);
                Debug.LogWarning(ex);
            }
        }
    }

    private int temp_screenModeIdx, temp_resolutionIdx;
    private float temp_bgm, temp_sfx;

    // Start is called before the first frame update
    void Start()
    {
        resolutions = new List<Resolution>(Screen.resolutions);
        btn_ScreenModeLeft.onClick.AddListener(ScreenModeLeft);
        btn_ScreenModeRight.onClick.AddListener(ScreenModeRight);
        btn_ResolutionLeft.onClick.AddListener(ResolutionLeft);
        btn_ResolutionRight.onClick.AddListener(ResolutionRight);
        sld_BgmSlider.onValueChanged.AddListener(SetBgmVolume);
        sld_SfxSlider.onValueChanged.AddListener(SetBgmVolume);
        btn_CopyEmail.onClick.AddListener(CopyEmail);

        ScreenModeIdx = 0;
        ResolutionIdx = 0;
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

    private void ScreenModeLeft()
    {
        ScreenModeIdx = (screenModeIdx + Enum.GetValues(typeof(ScreenMode)).Length - 1) % (Enum.GetValues(typeof(ScreenMode)).Length);
    }

    private void ScreenModeRight()
    {
        ScreenModeIdx = (screenModeIdx  + 1) % (Enum.GetValues(typeof(ScreenMode)).Length);
    }

    private void ResolutionLeft()
    {
        ResolutionIdx = (resolutionIdx + resolutions.Count - 1) % (resolutions.Count);
    }

    private void ResolutionRight()
    {
        ResolutionIdx = (resolutionIdx  + 1) % (resolutions.Count);
    }

    private void SetBgmVolume(float value)
    {

    }

    private void SetSfxVolume(float value)
    {

    }

    private void CopyEmail()
    {
        GUIUtility.systemCopyBuffer = "gangtoesal@gmail.com";
    }

    private void ConfirmSetting()
    {
        Hide();
    }

    private void CancelSetting()
    {
        ScreenModeIdx = temp_screenModeIdx;
        ResolutionIdx = temp_resolutionIdx;
        sld_BgmSlider.value = temp_bgm;
        sld_SfxSlider.value = temp_sfx;
        Hide();
    }
}
