using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SettingPopup : MonoBehaviour
{
    [SerializeField] private Toggle autoLoginCheckBox; 
    
    public void BackBtnClick()
    {
        UIManager.Instance.PopupListPop();
    }

    public void AutoLoginToggle()
    {
        int value = autoLoginCheckBox.isOn ? 1 : 0;

        PlayerPrefs.SetInt("NotAutoLogin", value);
        
        BackendManager.Instance.NotAutoLogin = autoLoginCheckBox.isOn;
    }
}
