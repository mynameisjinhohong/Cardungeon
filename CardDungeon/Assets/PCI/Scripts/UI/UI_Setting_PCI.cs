using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Setting_PCI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI txt_ScreenModeTxt;
    public Button btn_ScreenModeLeft, btn_ScreenModeRight;
    public TMPro.TextMeshProUGUI txt_ResolutionTxt;
    public Button btn_ResolutionLeft, btn_ResolutionRight;
    public Slider sld_BgmSlider, sld_SfxSlider;
    public Button btn_Confirm, btn_Cancel;
    public Toggle toggle_AutoLogin;

    public GameObject screenModeObj;
    public GameObject resolutionObj;
    public GameObject googleLogoutObj;
    
    private FullScreenMode screenMode;
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
                case (int)FullScreenMode.Windowed:
                    txt_ScreenModeTxt.text = "창모드";
                    break;
                case (int)FullScreenMode.FullScreenWindow:
                    txt_ScreenModeTxt.text = "전체 창모드";
                    break;
                case (int)FullScreenMode.ExclusiveFullScreen:
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
                txt_ResolutionTxt.text = resolutions[resolutionIdx].width + " x " + resolutions[resolutionIdx].height;
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
        toggle_AutoLogin.isOn = PlayerPrefs.GetInt("UseAutoLogin") == 1;
        btn_ScreenModeLeft.onClick.AddListener(ScreenModeLeft);
        btn_ScreenModeRight.onClick.AddListener(ScreenModeRight);
        btn_ResolutionLeft.onClick.AddListener(ResolutionLeft);
        btn_ResolutionRight.onClick.AddListener(ResolutionRight);
        sld_BgmSlider.onValueChanged.AddListener(SetBgmVolume);
        sld_SfxSlider.onValueChanged.AddListener(SetSfxVolume);
        btn_Confirm.onClick.AddListener(ConfirmSetting);
        btn_Cancel.onClick.AddListener(CancelSetting);
        toggle_AutoLogin.onValueChanged.AddListener(AutoLoginSetting);

        ScreenModeIdx = 0;
        ResolutionIdx = 0;
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height && resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                ResolutionIdx = i;
            }
        }

        #if PLATFORM_STANDALONE_WIN
        screenModeObj.SetActive(true);
        resolutionObj.SetActive(true);
        #elif PLATFORM_ANDROID
        screenModeObj.SetActive(false);
        resolutionObj.SetActive(false);
        #endif

        //googleLogoutObj.SetActive(BackendManager.Instance.loginType == LoginType.Google);
    }

    private void OnEnable()
    {
        resolutions = new List<Resolution>(Screen.resolutions);

        sld_BgmSlider.value = AudioPlayer.Instance.bgmPlayer.volume;
        temp_bgm = sld_BgmSlider.value;

        sld_SfxSlider.value = AudioPlayer.Instance.sfxPlayer.volume;
        temp_sfx = sld_SfxSlider.value;

        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height && resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                ResolutionIdx = i;
            }
        }
        temp_resolutionIdx = resolutionIdx;

        screenModeIdx = (int)Screen.fullScreenMode;
        switch (screenModeIdx)
        {
            case (int)FullScreenMode.Windowed:
                txt_ScreenModeTxt.text = "창모드";
                break;
            case (int)FullScreenMode.FullScreenWindow:
                txt_ScreenModeTxt.text = "전체 창모드";
                break;
            case (int)FullScreenMode.ExclusiveFullScreen:
                txt_ScreenModeTxt.text = "전체화면";
                break;
        }
        temp_screenModeIdx = screenModeIdx;
    }

    private void ScreenModeLeft()
    {
        ScreenModeIdx = (screenModeIdx + Enum.GetValues(typeof(FullScreenMode)).Length - 1) % (Enum.GetValues(typeof(FullScreenMode)).Length);
    }

    private void ScreenModeRight()
    {
        ScreenModeIdx = (screenModeIdx  + 1) % (Enum.GetValues(typeof(FullScreenMode)).Length);
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
        AudioPlayer.Instance.bgmPlayer.volume = value;
    }

    private void SetSfxVolume(float value)
    {
        AudioPlayer.Instance.sfxPlayer.volume = value;
        AudioPlayer.Instance.PlayClip(1);
    }
    
    private void GoogleSignOut()
    {
        //GoogleSignIn.DefaultInstance.SignOut();
    }

    // private void CopyEmail()
    // {
    //     GUIUtility.systemCopyBuffer = "gangtoesal@gmail.com";
    // }

    private void ConfirmSetting()
    {
        #if PLATFORM_STANDALONE_WIN
            Screen.SetResolution(resolutions[resolutionIdx].width, resolutions[resolutionIdx].height, screenMode, resolutions[resolutionIdx].refreshRate);
        #endif
        
        UIManager.Instance.PopupListPop();
    }

    private void CancelSetting()
    {
        ScreenModeIdx = temp_screenModeIdx;
        ResolutionIdx = temp_resolutionIdx;
        sld_BgmSlider.value = temp_bgm;
        sld_SfxSlider.value = temp_sfx;
        
#if PLATFORM_STANDALONE_WIN
        Screen.SetResolution(resolutions[resolutionIdx].width, resolutions[resolutionIdx].height, screenMode, resolutions[resolutionIdx].refreshRate);
#endif
        
        AudioPlayer.Instance.bgmPlayer.volume = sld_BgmSlider.value;
        AudioPlayer.Instance.sfxPlayer.volume = sld_SfxSlider.value;
        
        UIManager.Instance.PopupListPop();
    }

    private void AutoLoginSetting(bool toggleValue)
    {
        int boolValue = toggleValue ? 1 : 0;
        
        PlayerPrefs.SetInt("UseAutoLogin", boolValue);
    }
}
